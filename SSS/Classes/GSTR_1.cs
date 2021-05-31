using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace SSS
{

    public  class GSTR_1
    {
        public string gstin { get; set; }
        public string fp { get; set; }
        public string version { get; set; }
        public string hash { get; set; }

        public  List<b2b> b2b { get; set; }
        public List<b2ba> b2ba { get; set; }
        public List<b2cl> b2cl { get; set; }
        public List<b2cla> b2cla { get; set; }
        public List<b2cs> b2cs { get; set; }
        public List<b2csa> b2csa { get; set; }
        public List<nil> nil { get; set; }
        public List<exp> exp { get; set; }
        public List<expa> expa { get; set; }
        public List<cdnr> cdnr { get; set; }
        public List<cdnra> cdnra { get; set; }
        public List<cdnur> cdnur { get; set; }
        public List<cdnura> cdnura { get; set; }
        public List<doc_issue> doc_issue { get; set; }
        public List<hsn> hsn { get; set; }
        public List<txpd> txpd { get; set; }
        public List<txpda> txpda { get; set; }
    }
    public class b2b_itm_det
    {
        public double txval { get; set; }
        public double rt { get; set; }
        public double iamt { get; set; }
        public double csamt { get; set; }

    }
    public class b2b_itms
    {
        public double num { get; set; }
        public List<b2b_itm_det> itm_det { get; set; }     

    }

    public class b2b_inv
    {
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string pos  { get; set; }
        public string rchrg { get; set; }
        //public double diff_percent { get; set; }
        public string inv_typ { get; set; }
        public List<b2b_itms> itms { get; set; }

    }
   
    public class b2b
    {
        public  string ctin { get; set; }
        public List<b2b_inv> inv { get; set; }
    }

    public class b2ba_inv
    {
        public string oinum { get; set; }
        public string oidt { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string pos { get; set; }
        public string rchrg { get; set; }
        public string diff_percent { get; set; }
        public string inv_typ { get; set; }
        public List<b2b_itms> itms { get; set; }

    }

    public class b2ba
    {
        public string ctin { get; set; }
        public List<b2ba_inv> inv { get; set; }
    }

    public class b2cl_inv
    {
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string inv_typ { get; set; }
        public List<b2b_itms> itms { get; set; }

    }
     public class b2cl
    {
        public  string pos { get; set; }
        public List<b2cl_inv> inv { get; set; }
    }

    public class b2cla_inv
    {
        public string oinum { get; set; }
        public string oidt { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }    
        public string inv_typ { get; set; }
        public List<b2b_itms> itms { get; set; }

    }
    public class b2cla
    {
        public string pos { get; set; }
        public List<b2cla_inv> inv { get; set; }
    }

    public class b2cs
    {
        public string sply_ty { get; set; }
        public double rt { get; set; }
        public string typ { get; set; }
        public string pos { get; set; }
        public double diff_percent { get; set; }
        public double txval { get; set; }        
        public double iamt { get; set; }
        public double csamt { get; set; }
    }

    public class b2csa_itms
    {
        public double rt { get; set; }
        public double txval { get; set; }
        public double iamt { get; set; }
        public double csamt { get; set; }
    }

    public class b2csa
    {
        public string omon { get; set; }
        public string sply_ty { get; set; }
        public string typ { get; set; }
        public string pos { get; set; }
        public List<b2csa_itms> itms { get; set; }
    }

    public class nil_inv
    {
        public string sply_ty { get; set; }
        public double expt_amt { get; set; }
        public double nil_amt { get; set; }
        public double ngsup_amt { get; set; }
    }

    public class nil
    {
        public List<nil_inv> inv { get; set; }
    }

    public class exp_inv
    {
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string sbpcode { get; set; }
        public string sbnum { get; set; }
        public string sbdt { get; set; }
        public List<b2b_itms> itms { get; set; }
    }

    public  class exp
    {
        string exp_typ { get; set; }
        public List<exp_inv> inv { get; set; }       
    }

    public class expa_inv
    {
        string oinum { get; set; }
        string oidt { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string sbpcode { get; set; }
        public string sbnum { get; set; }
        public string sbdt { get; set; }
        public List<b2b_itms> itms { get; set; }
    }

    public class expa
    {
        string exp_typ { get; set; }
        public List<expa_inv> inv { get; set; }
    }

    public class cdnr_nt
    {
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string inum { get; set; }
        public string ntty { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string p_gst { get; set; }       
        public List<b2b_itms> itms { get; set; }

    }

    public class cdnr
    {
        public string ctin { get; set; }
        public List<cdnr_nt> nt { get; set; }
    }

    public class cdnra_nt
    {
        public string ont_num { get; set; }
        public string ont_dt { get; set; }
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string inum { get; set; }
        public string ntty { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string p_gst { get; set; }
        public double diff_percent { get; set; }
        public List<b2b_itms> itms { get; set; }

    }

    public class cdnra
    {
        public string ctin { get; set; }
        public List<cdnra_nt> nt { get; set; }
    }

    public class at_itms
    {
        public double rt { get; set; }
        public double ad_amt { get; set; }
        public double iamt { get; set; }
        public double csamt { get; set; }
    }

    public class at
    {
        public string pos { get; set; }
        public string sply_ty { get; set; }
        public double diff_percent { get; set; }
        public List<at_itms> itms { get; set; }
    }

    public class ata
    {
        public  string omon { get; set; }
        public string pos { get; set; }
        public string sply_ty { get; set; }
        public double diff_percent { get; set; }
        public List<at_itms> itms { get; set; }
    }

    public class cdnur
    {
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string inum { get; set; }
        public string ntty { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string p_gst { get; set; }
        public string typ { get; set; }
        public double diff_percent { get; set; }
        public List<b2b_itms> itms { get; set; }

    }

    public class cdnura
    {
        public string ont_num { get; set; }
        public string ont_dt { get; set; }
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public string inum { get; set; }
        public string ntty { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string p_gst { get; set; }
        public string typ { get; set; }
        public double diff_percent { get; set; }
        public List<b2b_itms> itms { get; set; }

    }

public class doc_issue_docs
    {
        public int num { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public double totnum { get; set; }
        public double cancel { get; set; }
        public double net_issue { get; set; }
    }

    public class doc_det
    {
        public int doc_num { get; set; }
        public string doc_typ { get; set; }
        public List<doc_issue_docs> docs { get; set; }
    }
    public class doc_issue
    {
        public List<doc_det> doc_det { get; set; }

    }

    public class hsn_data
    {
        public int num { get; set; }
        public string hsn_sc { get; set; }
        public string desc { get; set; }
        public string uqc { get; set; }
        public double qty { get; set; }
        public double val { get; set; }
        public double txval { get; set; }
        public double iamt { get; set; }
        public double samt { get; set; }
        public double camt { get; set; }
        public double csamt { get; set; }
    }

    public class hsn
    {
        public List<hsn_data> data { get; set; }
    }

    public  class txpd
    {
        public string pos { get; set; }
        public string sply_ty { get; set; }
        public double diff_percent { get; set; }
        public List<at_itms> itms { get; set; }
    }

    public class txpda
    {
        public string omon { get; set; }
        public string pos { get; set; }
        public string sply_ty { get; set; }
        public double diff_percent { get; set; }
        public List<at_itms> itms { get; set; }
    }


}

