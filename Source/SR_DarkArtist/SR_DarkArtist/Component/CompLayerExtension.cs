using RimWorld;
using Verse;

namespace SR.DA.Component
{
    /// <summary>
    /// layer扩展组件 多layer
    /// </summary>
    public class CompLayerExtension : ThingComp
    {
        public CompProperties_LayerExtension Props
        {
            get
            {
                return (CompProperties_LayerExtension)props;
            }
        }
        /// <summary>
        /// 绘制
        /// </summary>
        public override void PostDraw()
        {
            base.PostDraw();
            if (Props.gas == null || Props.gas.Count == 0) 
            {
                return;
            }
            for (int i = 0; i < Props.gas.Count; i++)
            {
                Props.gas[i].graphicData.Graphic.Draw(GenThing.TrueCenter(parent.Position, parent.Rotation, parent.def.size, Props.gas[i].altitudeLayer.AltitudeFor()), parent.Rotation, parent, 0f);
            }
        }
    }
}
