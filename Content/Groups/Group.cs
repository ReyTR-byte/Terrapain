using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static Terrapain.Content.Functions;

namespace Terrapain.Content.Groups
{
    public abstract class Group
    {
        public virtual string Name => GetType().Name;
        public int whoAmI = -1;
        // -1 is multitype
        public int[] NPCType;
        // -1 is no leader
        public int leader;
        public List<int> members = new List<int>();
        public List<float> sort = new List<float>();
        public int Count => members.Count;
        public bool active;
        /// <summary>
        /// retun count of removed members
        /// </summary>
        /// <returns></returns>
        public int CheckMembers()
        {
            int count = 0;
            for (int i = 0; i < Count; i++)
            {
                NPC member = Main.npc[members[i]];
                if (!member.active || (NPCType.Length != 0 && !NPCType.Contains(member.type)))
                {
                    DelMember(i);
                    i--;
                    count++;
                }
            }
            if (members.Count == 0)
            {
                Terrapain.group[whoAmI] = null;
                return count;
            }
            return count;
        }
        public virtual void UpdateGroup()
        {

        }
        public virtual void PostDrawNPCs(SpriteBatch spriteBatch, Vector2 screenPosition)
        {

        }
        public void AddMember(int member)
        {
            if (!members.Contains(member))
            {
                members.Add(member);
                sort.Add(0);
                Main.npc[member].GetT().MyGroups.Add(whoAmI);
            }
        }
        public void DelMember(int i)
        {
            if (Main.npc[members[i]].active)
            {
                Main.npc[members[i]].GetT().MyGroups.Remove(whoAmI);
            }
            members.RemoveAt(i);
            sort.RemoveAt(i);
        }
        int FindPivot(int minValue, int maxValue)
        {
            int pivot = minValue - 1;
            int tempNPC = 0;
            float tempSort = 0;
            for (int i = minValue; i < maxValue; i++)
            {
                if (sort[i] < sort[maxValue])
                {
                    pivot++;
                    tempNPC = members[pivot];
                    tempSort = sort[pivot];
                    members[pivot] = members[i];
                    sort[pivot] = sort[i];
                    members[i] = tempNPC;
                    sort[i] = tempSort;
                    string _sort = "";
                    string _members = "";
                    for (int g = 0; g < Count; g++)
                    {
                        _sort += sort[g].ToString() + " ";
                        _members += members[g].ToString() + " ";
                    }
                    //Functions.Chatic(_sort, _members, pivot);
                }
            }
            pivot++;
            tempNPC = members[pivot];
            tempSort = sort[pivot];
            members[pivot] = members[maxValue];
            sort[pivot] = sort[maxValue];
            members[maxValue] = tempNPC;
            sort[maxValue] = tempSort;
            string __sort = "";
            string __members = "";
            for (int i = 0; i < Count; i++)
            {
                __sort += sort[i].ToString() + " ";
                __members += members[i].ToString() + " ";
            }
            //Functions.Chatic(__sort, __members, pivot);
            //Functions.Chatic(pivot);
            return pivot;
        }
        public void Sort()
        {
            while(sort.Count < members.Count)
            {
                sort.Add(0);
            }
            Sort(0, members.Count - 1);
        }
        void Sort(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
            {
                return;
            }
            int pivot = FindPivot(minValue, maxValue);
            Sort(minValue, pivot - 1);
            Sort(pivot + 1, maxValue);
        }
        public static int NewGroup(Group group)
        {
            for (int i = 0; i < Terrapain.group.Length; i++)
            {
                if (Terrapain.group[i] == null)
                {
                    group.active = true;
                    group.whoAmI = i;
                    Terrapain.group[i] = group;
                    return group.whoAmI;
                }
            }
            return -1;
        }
        public static List<int> FindGroup(string Name)
        {
            List<int> groups = new List<int>();
            foreach(var group in Terrapain.group)
            {
                if (group != null && group.Name == Name)
                {
                    groups.Add(group.whoAmI);
                }
            }
            return groups;
        }
    }
}
