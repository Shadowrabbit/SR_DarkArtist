using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace SR.DA.Thing
{
    public class Building_ElectricChair : Building
    {
        private bool isUsing = false;
        private static readonly float workingPower = 5000f;//工作耗电,使用是会给电力系统增加负荷
        private CompPowerTrader cpt;
        private CompFlickable cf;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref isUsing, "isUsing", false);
            //Scribe_References.Look(ref occupant, "occupant");
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            cpt = GetComp<CompPowerTrader>();
            cf= GetComp<CompFlickable>();
            OnPowerChanged();
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
        public override void Tick()
        {
            base.Tick();
        }
        /// <summary>
        /// 设置使用状态
        /// </summary>
        /// <param name="b"></param>
        public void OnOrOff(bool b) {
            isUsing = b;
            OnPowerChanged();
        }
        private void OnPowerChanged() {
            //耗电量开启电源的话取决于是否工作 关闭电源为0
            cpt.PowerOutput = cpt.PowerOn ? isUsing ? -workingPower : -cpt.Props.basePowerConsumption : 0f;
        }
    }
}
