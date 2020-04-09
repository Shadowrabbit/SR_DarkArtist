using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using SR.DA.Component;

namespace SR.DA.Thing
{
    public class Building_BondageBed : Building_Bed
    {
        public Pawn occupant;//使用者
        private static readonly Color SheetColorMedicalForPrisoner = new Color(0.654902f, 0.3764706f, 0.152941182f);
        public override Color DrawColorTwo => SheetColorMedicalForPrisoner;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref occupant, "occupant");//保存当前使用者
        }
        /// <summary>
        /// 绘制囚犯房间边缘
        /// </summary>
        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
        }
        /// <summary>
        /// 绘制命令
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Gizmo> GetGizmos()
        {
            def.building.bed_humanlike = false;//高度分化，没办法调用祖父类，为了阻止绘制医疗/囚犯/拥有者命令，尝试让其不满足父类执行条件
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            def.building.bed_humanlike = true;
        }
        /// <summary>
        /// label绘制
        /// </summary>
        public override void DrawGUIOverlay()
        {
            if (occupant != null)
            {
                Color defaultThingLabelColor = Color.yellow;
                GenMapUI.DrawThingLabel(this, "SR_Bound".Translate(), defaultThingLabelColor);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
			if (this.def.building.bed_humanlike)
			{
                stringBuilder.AppendInNewLine("ForPrisonerUse".Translate());
			}
			return stringBuilder.ToString();
        }
        /// <summary>
        /// 安装
        /// </summary>
        /// <param name="map"></param>
        /// <param name="respawningAfterLoad"></param>
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            Medical = false;//禁止医用
            ForPrisoners = true;//囚犯专用
        }
        /// <summary>
        /// 拆卸
        /// </summary>
        /// <param name="mode"></param>
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            //如果床上面有囚犯
            if (occupant!=null)
            {
                var crebb = GetComp<CompRemoveEffectBondageBed>() ?? throw new Exception("cant find comp : CompRemoveEffectBondageBed");
                crebb.DoEffect(occupant);//解除使用者
            }
            base.DeSpawn(mode);
            //Room room = this.GetRoom(RegionType.Set_Passable);
            //if (room != null)
            //{
            //    room.Notify_RoomShapeOrContainedBedsChanged();
            //}
        }
        /// <summary>
        /// 设置使用者
        /// </summary>
        /// <param name="occupant"></param>
        public void SetOccupant(Pawn occupant) {
            if (occupant!=null)
            {
                this.occupant = occupant;
                OwnersForReading.Clear();
                OwnersForReading.Add(occupant);
            }
        }
        /// <summary>
        /// 移除使用者
        /// </summary>
        public void RemoveOccupant() {
            occupant = null;
            OwnersForReading.Clear();
        }
        /// <summary>
        /// 绘制选项
        /// </summary>
        /// <param name="myPawn"></param>
        /// <returns></returns>
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            if (AllComps != null)
            {
                for (int i = 0; i < AllComps.Count; i++)
                {
                    foreach (FloatMenuOption floatMenuOption in AllComps[i].CompFloatMenuOptions(myPawn))
                    {
                        yield return floatMenuOption;
                    }
                }
            }
        }
    }
}
