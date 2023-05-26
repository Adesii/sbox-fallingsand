using Sand.Systems.FallingSand;
using Sandbox;
using Sandbox.UI;


public partial class ElementSelector : Panel
{

	public List<Type> Elements { get; set; } = new();
	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );
		if ( !firstTime ) return;


		Elements = TypeLibrary.GetTypesWithAttribute<ElementAttribute>().Select( x => x.Type.TargetType ).ToList();
	}


	protected override int BuildHash()
	{
		return base.BuildHash() ^ Elements.Count;
	}

	public void NotifyElementChange( Type NewElement, bool LeftClick )
	{
		CreateEvent( LeftClick ? "selectedLeftclick" : "selectedRightclick", NewElement );
	}
}
