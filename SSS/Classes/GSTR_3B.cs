using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SSS
{
    class GSTR_3B
    {
        public  string gstin { get; set; }
        public string ret_period { get; set; }
        public sup_details sup_details { get; set; }
        public itc_elg itc_elg { get; set; }
        public inward_sup inward_sup { get; set; }

        public intr_ltfee intr_ltfee { get; set; }

        public inter_sup inter_sup { get; set; }        

    }
    public class osup_det
    {
        public double txval { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }

    }
    

    public class osup_zero
    {
        public double txval { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }

    public class osup_nil_exmp
    {
        public double txval { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }

    public class isup_rev
    {
        public double txval { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }

    public class osup_nongst
    {
        public double txval { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }
    public class sup_details
    {
        public osup_det osup_det { get; set; }
        public osup_zero osup_zero { get; set; }
        public osup_nil_exmp osup_nil_exmp { get; set; }
        public isup_rev isup_rev { get; set; }
        public osup_nongst osup_nongst { get; set; }
    }

    public class itc_avl
    {
        public string ty { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }
    public class itc_rev
    {
        public string ty { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }
    public class itc_net
    {
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }


    public class itc_inelg
    {
        public string ty { get; set; }
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }


    public class itc_elg
    {
        public List<itc_avl> itc_avl { get; set; }
        public List<itc_rev> itc_rev { get; set; }
        public List<itc_net> itc_net { get; set; }
        public List<itc_inelg> itc_inelg { get; set; }
    }

    public class isup_details
    {
        public string ty { get; set; }
        public double inter { get; set; }
        public double intra { get; set; }
    }

    public class inward_sup
    {
        public List<isup_details> isup_details { get; set; }
    }
    
    public class intr_details
    {
        public double iamt { get; set; }
        public double camt { get; set; }
        public double samt { get; set; }
        public double csamt { get; set; }
    }

    public class ltfee_details
    {
        
    }
    public class intr_ltfee
    {
        public List<intr_details> intr_details { get; set; }
        public List<ltfee_details> ltfee_details { get; set; }
    }

    public class unreg_details
    { }
    public class comp_details
    { }
    public class uin_details
    { }
    public class inter_sup
    {
        public List<unreg_details> unreg_details { get; set; }
        public List<comp_details> comp_details { get; set; }
        public List<uin_details> uin_details { get; set; }
    }
}
