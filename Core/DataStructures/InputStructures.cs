using FinalSnack.Common;

namespace FinalSnack.Core.DataStructures
{
    public struct KeyboardMouseInput
    {
        public Keys KeyboardInput = Keys.None;
        public MouseClickType MouseInput = MouseClickType.None;
        public KeyboardMouseInput(Keys keyboard, MouseClickType mouse)
        {
            KeyboardInput = keyboard;
            MouseInput = mouse;
        }
        public KeyboardMouseInput(Keys keyboard)
        {
            KeyboardInput = keyboard;
        }
        public KeyboardMouseInput(MouseClickType mouse)
        {
            MouseInput = mouse;
        }
    }
    public struct GameInput(KeyboardMouseInput computerInput, GamePadState gamepadInput)
    {
        public KeyboardMouseInput ComputerInput = computerInput;
        public GamePadState GamePadInput = gamepadInput;
    }
    public struct CompressedGameInput(GameInput input, float length)
    {
        public GameInput Input = input;
        public float Length = length;
    }
}
