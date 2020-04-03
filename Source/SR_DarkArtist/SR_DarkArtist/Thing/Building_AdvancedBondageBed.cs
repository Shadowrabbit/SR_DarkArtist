using System.Text;
using UnityEngine;
using Verse;

namespace SR.DA.Thing
{
    public class Building_AdvancedBondageBed : Building_BondageBed
    {
        private static readonly Color SheetColorNormal = new Color(0.6313726f, 0.8352941f, 0.7058824f);
        public override Color DrawColor => SheetColorNormal;
        public override Color DrawColorTwo => SheetColorNormal;
        /// <summary>
        /// label绘制
        /// </summary>
        public override void DrawGUIOverlay()
        {
            if (occupant != null)
            {
                Color defaultThingLabelColor = Color.green;
                GenMapUI.DrawThingLabel(this, "SR_Bound".Translate(), defaultThingLabelColor);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.def.building.bed_humanlike)
            {
                stringBuilder.AppendInNewLine("ForColonistUse".Translate());
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 生成回调
        /// </summary>
        /// <param name="map"></param>
        /// <param name="respawningAfterLoad"></param>
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            Medical = false;
            ForPrisoners = false ;
        }
    }
}
