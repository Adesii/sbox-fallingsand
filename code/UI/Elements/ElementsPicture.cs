using Sandbox;
using Sandbox.UI;


public partial class ElementsPicture : Panel
{
	public Type Element { get; set; }

	public Panel activepanel { get; set; }


	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );
		if ( !firstTime ) return;
		if ( Sand.UI.Hud.LeftClick == Element )
		{
			activepanel.SetClass( "active-blue", true );
		}
		if ( Sand.UI.Hud.RightClick == Element )
		{
			activepanel.SetClass( "active-red", true );
		}
	}

	bool hasleftclick => activepanel.HasClass( "active-blue" );
	bool hasrightclick => activepanel.HasClass( "active-red" );

	bool hasboth => activepanel.HasClass( "active-both" );

	[Event( "ElementSelectionChanged" )]
	public void ElementSelectionChanged( Type NewElement, bool LeftClick )
	{
		if ( NewElement == Element )
		{
			if ( LeftClick && hasrightclick )
			{
				activepanel.SetClass( "active-both", true );
			}
			else if ( !LeftClick && hasleftclick )
			{
				activepanel.SetClass( "active-both", true );
			}
			else
			{
				activepanel.SetClass( "active-blue", LeftClick );
				activepanel.SetClass( "active-red", !LeftClick );
			}
			return;
		}
		else if ( hasleftclick && LeftClick )
		{
			activepanel.SetClass( "active-blue", false );
		}
		else if ( hasrightclick && !LeftClick )
		{
			activepanel.SetClass( "active-red", false );
		}
		if ( hasboth && !LeftClick )
		{
			activepanel.SetClass( "active-blue", true );
			activepanel.SetClass( "active-both", false );
		}
		else if ( hasboth && LeftClick )
		{
			activepanel.SetClass( "active-red", true );
			activepanel.SetClass( "active-both", false );
		}

	}


	protected override void OnClick( MousePanelEvent e )
	{
		Sand.UI.Hud.LeftClick = Element;
		Event.Run( "ElementSelectionChanged", Element, true );
	}

	protected override void OnRightClick( MousePanelEvent e )
	{
		Sand.UI.Hud.RightClick = Element;
		Event.Run( "ElementSelectionChanged", Element, false );
	}
}
