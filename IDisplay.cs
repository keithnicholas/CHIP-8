using System;

public interface IDisplay
{
    public void ClearDisplay();
    public byte GetDisplayAtCoord(int x, int y);

}
