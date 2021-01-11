using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class UtilityList
{
    int tid;
    public int TID
    {
        get { return tid; }
        set { tid = value; }
    }

    float acu;
    public float ACU
    {
        get { return acu; }
        set { acu = value; }
    }
    UtilityList link;
    public UtilityList Link
    {
        get { return link; }
        set { link = value; }
    }
    //Constructor
    public UtilityList() { }
    public UtilityList(int tid, float acu, UtilityList link = null)
    {
        this.tid = tid;
        this.acu = acu;
        this.link = link;
    }
}
