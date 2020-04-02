using Verse;
using System.Collections.Generic;

namespace SR.DA.Component
{
    /// <summary>
    /// 多layer组件参数
    /// </summary>
    public class CompProperties_LayerExtension : CompProperties
    {
        public class GA {
            public GraphicData graphicData;//额外的图形
            public AltitudeLayer altitudeLayer;//对应的layer层
        }
        public CompProperties_LayerExtension()
        {
            compClass = typeof(CompLayerExtension);
        }
        public List<GA> gas;
    }
}
