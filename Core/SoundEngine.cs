using FmodForFoxes.Studio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FinalSnack.Core
{
    public readonly record struct PlaySoundParameters(Vector2? Position, float? Volume, float? Pan, float? Pitch)
    {
        public static PlaySoundParameters None { get; private set; } = default;
        public static PlaySoundParameters FromPosition(Vector2 relativeToListener)
        {
            return new PlaySoundParameters
            (

            );
        }
    };
    public class StreamedFileAbstraction(Stream stream) : TagLib.File.IFileAbstraction
    {
        private readonly Stream _stream = stream;
        string TagLib.File.IFileAbstraction.Name => "STREAMED.ogg";

        Stream TagLib.File.IFileAbstraction.ReadStream => _stream;

        Stream TagLib.File.IFileAbstraction.WriteStream => _stream;

        void TagLib.File.IFileAbstraction.CloseStream(Stream stream)
        {

        }
    }
    public static class SoundEngine
    {
        private const string OGG = ".ogg";
        private static readonly Dictionary<string, Sound> _soundEffectsRegistry = [];
        public static void ClearCache() => _soundEffectsRegistry.Clear();
        /// <summary>
        /// Plays a sound with the given name and parameters.
        /// </summary>
        /// <param name="path">The path to the sound that needs to be played.</param>
        /// <param name="parameters">The <see cref="PlaySoundParameters"/> of the sound that needs to be played.</param>
        /// <param name="lookForLoopTags">Whether or not this sound has special OGG loop tags that should be used.</param>
        /// <param name="register">Whether or not this sound should be cached for faster usage. Set this to true for sounds that will be played a lot.</param>
        /// <returns>The <see cref="Channel"/> instance that was just played.</returns>
        public static Channel PlaySound(string path, PlaySoundParameters parameters = default, bool lookForLoopTags = false, bool register = false)
        {
            Sound inst;

            if (_soundEffectsRegistry.TryGetValue(path, out var cached))
            {
                inst = cached;
            }
            else
            {
                inst = LoadSound(path, false, lookForLoopTags);
                if (register)
                    _soundEffectsRegistry[path] = inst;
            }

            inst.Is3D = false;
            inst.Mode = FMOD.MODE._2D;
            return inst.Play();
        }
        /// <summary>
        /// Loads a sound but good (actually respects OGG tags).
        /// </summary>
        /// <param name="path">The relative path (without extension) to the sound.</param>
        /// <param name="streamed">Whether or not this should be loaded as a streamed sound.</param>
        /// <param name="specialLooping">Whether or not this sound should check for special looping tags when loading. By default, this will be true if <paramref name="streamed"/> is true. Slightly increases load times.</param>
        /// <returns></returns>
        public static Sound LoadSound(string path, bool streamed = false, bool? specialLooping = null)
        {
            Sound s = streamed ? CoreSystem.LoadStreamedSound(path + OGG) : CoreSystem.LoadSound(path + OGG);

            if (specialLooping ?? streamed)
            {
                (uint? start, uint? end) = GetLoopPointsFromOGGFile(path + OGG);
                s.Native.getLoopPoints(out uint origStart, FMOD.TIMEUNIT.PCM, out uint origEnd, FMOD.TIMEUNIT.PCM);
                s.Native.setLoopPoints(start ?? origStart, FMOD.TIMEUNIT.PCM, end ?? origEnd, FMOD.TIMEUNIT.PCM);
            }

            return s;
        }
        private static (uint?, uint?) GetLoopPointsFromOGGFile(string path)
        {
            long start = -1, end = -1;
            string finalPath = Path.Combine(FileLoader.RootDirectory, path);

            using var stream = TitleContainer.OpenStream(finalPath);
            var file = TagLib.File.Create(new StreamedFileAbstraction(stream));

            if (file.GetTag(TagLib.TagTypes.Xiph, false) is TagLib.Ogg.XiphComment xiph)
            {
                var possibleLoopStart = xiph.GetField("LOOPSTART");
                var possibleLoopEnd = xiph.GetField("LOOPEND");

                if (possibleLoopStart?.Length > 0)
                    if (uint.TryParse(possibleLoopStart[0], out uint ls))
                        start = ls;
                if (possibleLoopEnd?.Length > 0)
                    if (uint.TryParse(possibleLoopEnd[0], out uint le))
                        end = le;
            }
            else
            {
                throw new InvalidDataException("The provided file doesn't have valid XiphComment block. Is this not an .ogg file?");
            }

            return (start < 0 ? null : (uint)start, end < 0 ? null : (uint)end);
        }
    }
}
