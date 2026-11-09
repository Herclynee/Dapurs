using UnityEngine.InputSystem;

public static class MenuInput
{
    static Keyboard K => Keyboard.current;
    static Gamepad G => Gamepad.current;

    // Helper untuk cek banyak tombol sekaligus
    static bool Pressed(params Key[] keys)
    {
        if (K == null) return false;
        foreach (var k in keys)
        {
            if (K[k].wasPressedThisFrame) return true;
        }
        return false;
    }

    public static bool Up      => Pressed(Key.UpArrow, Key.W) || (G != null && G.dpad.up.wasPressedThisFrame);
    public static bool Down    => Pressed(Key.DownArrow, Key.S) || (G != null && G.dpad.down.wasPressedThisFrame);
    public static bool Left    => Pressed(Key.LeftArrow, Key.A) || (G != null && G.dpad.left.wasPressedThisFrame);
    public static bool Right   => Pressed(Key.RightArrow, Key.D) || (G != null && G.dpad.right.wasPressedThisFrame);
    
    // E / Spasi / Enter / Z = Pilih
    public static bool Confirm => Pressed(Key.E, Key.Space, Key.Enter, Key.NumpadEnter, Key.Z) 
                                  || (G != null && G.buttonSouth.wasPressedThisFrame);
                                  
    // Esc / X / Backspace = Batal
    public static bool Cancel  => Pressed(Key.Escape, Key.X, Key.Backspace) 
                                  || (G != null && G.buttonEast.wasPressedThisFrame);
                                  
    public static bool Mute    => Pressed(Key.M);
}