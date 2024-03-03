public class Vertices
{
    private int tl; // top left     - index 0
    private int tr; // top right    - index 1
    private int bl; // bottom left  - index 2
    private int br; // bottom right - index 3
    public readonly int size;

    public Vertices()
    {
        tl = -1;
        tr = -1;
        bl = -1;
        br = -1;

        size = 4;
    }

    public int Get(int index)
    {
        if (index == 0) return tl;
        else if (index == 1) return tr;
        else if (index == 2) return bl;
        else if (index == 3) return br;
        else return -1;
    }

    public void Set(int index, int value)
    {
        if (index == 0) tl = value;
        else if (index == 1) tr = value;
        else if (index == 2) bl = value;
        else if (index == 3) br = value;
    }
}

public class Tile
{
    public int x;
    public int y;
    public Vertices vertices; // inside and outside vertices
    public string name;

    public Tile(int x_, int y_, Vertices vertices_)
    {
        x = x_;
        y = y_;
        vertices = vertices_;
        name = "";
    }
}
