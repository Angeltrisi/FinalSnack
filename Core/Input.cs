using FinalSnack.Common;
using FinalSnack.Core.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FinalSnack.Core
{
    public static class Input
    {
        public static KeyboardState KeyCur { get; private set; }
        public static KeyboardState KeyPrev { get; private set; }
        public static MouseState MouseCur { get; private set; }
        public static MouseState MousePrev { get; private set; }
        public static Rectangle MouseRect => new(MouseCur.Position, new Point(1));
        public static Vector2 MouseDelta => (MouseCur.Position - MousePrev.Position).ToVector2();
        public static GamePadState GamepadCur { get; private set; }
        public static GamePadState GamepadPrev { get; private set; }
        public static bool GamepadIsConnected { get; private set; }
        public static bool UsingGamepad { get; private set; }
        public static float usingGamepadTimer = 0f;
        private const float gamepadTimerSeconds = 1f;
        public static bool OneShotPress(KeyboardMouseInput computerInput)
        {
            return Main.GameFocused && (OneShotClick(computerInput.MouseInput) || OneShotKeyPress(computerInput.KeyboardInput));
        }
        public static bool OneShotRelease(KeyboardMouseInput computerInput)
        {
            return Main.GameFocused && (OneShotMouseRelease(computerInput.MouseInput) || OneShotKeyRelease(computerInput.KeyboardInput));
        }
        public static bool HoldPress(KeyboardMouseInput computerInput)
        {
            return Main.GameFocused && (HoldClick(computerInput.MouseInput) || HoldKeyPress(computerInput.KeyboardInput));
        }
        private static bool OneShotKeyPress(Keys key) => KeyCur.IsKeyDown(key) && !KeyPrev.IsKeyDown(key);

        private static bool HoldKeyPress(Keys key) => KeyCur.IsKeyDown(key);

        private static bool OneShotKeyRelease(Keys key) => KeyPrev.IsKeyDown(key) && !KeyCur.IsKeyDown(key);

        private static bool OneShotClick(MouseClickType button)
        {
            return button switch
            {
                MouseClickType.Left => MouseCur.LeftButton == ButtonState.Pressed && MousePrev.LeftButton == ButtonState.Released,
                MouseClickType.Right => MouseCur.RightButton == ButtonState.Pressed && MousePrev.RightButton == ButtonState.Released,
                MouseClickType.Middle => MouseCur.MiddleButton == ButtonState.Pressed && MousePrev.MiddleButton == ButtonState.Released,
                MouseClickType.Button4 => MouseCur.XButton1 == ButtonState.Pressed && MousePrev.XButton1 == ButtonState.Released,
                MouseClickType.Button5 => MouseCur.XButton2 == ButtonState.Pressed && MousePrev.XButton2 == ButtonState.Released,
                _ => false,
            };
        }

        private static bool OneShotMouseRelease(MouseClickType button)
        {
            return button switch
            {
                MouseClickType.Left => MouseCur.LeftButton == ButtonState.Released && MousePrev.LeftButton == ButtonState.Pressed,
                MouseClickType.Right => MouseCur.RightButton == ButtonState.Released && MousePrev.RightButton == ButtonState.Pressed,
                MouseClickType.Middle => MouseCur.MiddleButton == ButtonState.Released && MousePrev.MiddleButton == ButtonState.Pressed,
                MouseClickType.Button4 => MouseCur.XButton1 == ButtonState.Released && MousePrev.XButton1 == ButtonState.Pressed,
                MouseClickType.Button5 => MouseCur.XButton2 == ButtonState.Released && MousePrev.XButton2 == ButtonState.Pressed,
                _ => false,
            };
        }

        private static bool HoldClick(MouseClickType button)
        {
            return button switch
            {
                MouseClickType.Left => MouseCur.LeftButton == ButtonState.Pressed,
                MouseClickType.Right => MouseCur.RightButton == ButtonState.Pressed,
                MouseClickType.Middle => MouseCur.MiddleButton == ButtonState.Pressed,
                MouseClickType.Button4 => MouseCur.XButton1 == ButtonState.Pressed,
                MouseClickType.Button5 => MouseCur.XButton2 == ButtonState.Pressed,
                _ => false
            };
        }
        public static bool JustScrolled(out int value)
        {
            if (Main.GameFocused)
            {
                int prevValue = MousePrev.ScrollWheelValue;
                int curValue = MouseCur.ScrollWheelValue;
                if (prevValue > curValue)
                {
                    value = -1;
                    return true;
                }
                else if (prevValue < curValue)
                {
                    value = 1;
                    return true;
                }
            }
            value = 0;
            return false;
        }
        public static bool OneShotControllerPress(Buttons button)
        {
            return Main.GameFocused && GamepadCur.IsButtonDown(button) && !GamepadPrev.IsButtonDown(button);
        }
        public static bool ControllerHoldPress(Buttons button)
        {
            return Main.GameFocused && GamepadCur.IsButtonDown(button);
        }
        public static float HoldTrigger(ControllerTriggerType trigger)
        {
            if (trigger == ControllerTriggerType.Left)
            {
                return GamepadCur.Triggers.Left;
            }
            if (trigger == ControllerTriggerType.Right)
            {
                return GamepadCur.Triggers.Right;
            }
            return 0;
        }

        public static Vector2 Triggers()
        {
            return new Vector2(GamepadCur.Triggers.Left, GamepadCur.Triggers.Right);
        }

        public static Buttons[] GetPressedButtons()
        {
            List<Buttons> pressedButtons = [];

            foreach (Buttons btn in Enum.GetValues<Buttons>())
            {
                if (GamepadCur.IsButtonDown(btn))
                {
                    pressedButtons.Add(btn);
                }
            }
            return [.. pressedButtons];
        }

        public static bool HasInputChanged()
        {
            GamePadState cur = GamepadCur;

            var emptyGamepadState = new GamePadState(Vector2.Zero, Vector2.Zero, 0, 0, Buttons.None);
            var currentGamepadState = new GamePadState(cur.ThumbSticks.Left, cur.ThumbSticks.Right, cur.Triggers.Left, cur.Triggers.Right, Accessors.CallGetVirtualButtons(ref cur));
            return currentGamepadState != emptyGamepadState;
        }

        public static void Update(GameTime gt)
        {
            KeyPrev = KeyCur;
            KeyCur = Keyboard.GetState();
            MousePrev = MouseCur;
            MouseCur = Mouse.GetState();
            GamepadPrev = GamepadCur;
            GamepadCur = GamePad.GetState(0);
            GamepadIsConnected = GamepadCur.IsConnected;
            if (GamepadIsConnected)
            {
                if (HasInputChanged())
                {
                    usingGamepadTimer = gamepadTimerSeconds;
                }
            }
            if (usingGamepadTimer > 0f)
            {
                UsingGamepad = true;
                usingGamepadTimer -= gt.Delta();
            }
            else
            {
                UsingGamepad = false;
            }
        }
        public static class Accessors
        {
            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetVirtualButtons")]
            public static extern Buttons CallGetVirtualButtons(ref GamePadState instance);
        }
    }
}
