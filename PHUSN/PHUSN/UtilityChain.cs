using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UtilityChain
{

   float peuts;
    public float PEUTS
    {
        get { return peuts; }
        set { peuts = value; }
    }

    UtilityList utilityList;
    public UtilityList UtilityList
    {
        get { return utilityList; }
        set { utilityList = value; }
    }
    public UtilityChain() { }
    public UtilityChain(float peuts, UtilityList utilityList)
    {
        this.peuts = peuts;
        this.utilityList = utilityList;
    }
}
