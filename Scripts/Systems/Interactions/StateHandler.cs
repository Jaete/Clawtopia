
using ClawtopiaCs.Scripts.Entities;
using Godot; 

namespace ClawtopiaCs.Scripts.Systems.Interactions;


public class Hover
{
    public Color Color()
    {
        return StateColors.HoverColor();
    }
}

public class Unhover
{
	public Color Color()
    {
        return StateColors.RegularColor();
    }
}

public class Error
{
	public Color Color()
	{
        return StateColors.ErrorColor();
	}
}

public class Ok 
{
	public Color Color()
	{
		return StateColors.OkColor();
	}
}


public class Finished
{
	public Color Color()
	{
		return StateColors.RegularColor();
	}
}



