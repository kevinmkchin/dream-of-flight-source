using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ruins.Source.Tool {
    public class Input {

        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;

        public static KeyboardState GetState() {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            return currentKeyState;
        }

        public static bool IsPressed(Keys key) {
            return currentKeyState.IsKeyDown(key);
        }

        public static bool HasBeenPressed(Keys key) {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        public static bool HasBeenReleased(Keys key) {
            return currentKeyState.IsKeyUp(key) && previousKeyState.IsKeyDown(key);
        }

        public static bool IsReleased(Keys key) {
            return currentKeyState.IsKeyUp(key);
        }

    }
}
