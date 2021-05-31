using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSS
{
   
    public class EInvoice
    {
        public string Version { get; set; }
        public TranDtls TranDtls { get; set; } = new TranDtls();
        public DocDtls DocDtls { get; set; } = new DocDtls();
        public SellerDtls SellerDtls { get; set; } = new SellerDtls();
        public BuyerDtls BuyerDtls { get; set; } = new BuyerDtls();
        public DispDtls DispDtls { get; set; } = new DispDtls();
        public ShipDtls ShipDtls { get; set; } = new ShipDtls();
        public ValDtls ValDtls { get; set; } = new ValDtls();
        public ExpDtls ExpDtls { get; set; } = new ExpDtls();
        public EwbDtls EwbDtls { get; set; } = new EwbDtls();
        public PayDtls PayDtls { get; set; } = new PayDtls();
        public RefDtls RefDtls { get; set; } = new RefDtls();
        public List<AddlDocDtls> AddlDocDtls { get; set; } = new List<AddlDocDtls>();
        public List<ItemsDtls> ItemList { get; set; } = new List<ItemsDtls>();
    }
    public class AddlDocDtls
    {
        public string Url { get; set; }
        public string Docs { get; set; }
        public string Info { get; set; }
    }
    public class ItemsDtls
    {
        public string SlNo { get; set; }
        public string PrdDesc { get; set; }
        public string IsServc { get; set; }
        public string HsnCd { get; set; }
        public string Barcde { get; set; }
        public double Qty { get; set; }
        public double FreeQty { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public double TotAmt { get; set; }
        public double Discount { get; set; }
        public double PreTaxVal { get; set; }
        public double AssAmt { get; set; }
        public double GstRt { get; set; }
        public double IgstAmt { get; set; }
        public double CgstAmt { get; set; }
        public double SgstAmt { get; set; }
        public double CesRt { get; set; }
        public double CesAmt { get; set; }
        public double CesNonAdvlAmt { get; set; }
        public double StateCesRt { get; set; }
        public double StateCesAmt { get; set; }
        public double StateCesNonAdvlAmt { get; set; }
        public double OthChrg { get; set; }
        public double TotItemVal { get; set; }
        public string OrdLineRef { get; set; }
        public string OrgCntry { get; set; }
        public string PrdSlNo { get; set; }
        public string BchDtls { get; set; }
        public List<AttribDtls> AttribDtls { get; set; } = new List<AttribDtls>();
    }
    public class AttribDtls
    {
        public string Nm { get; set; }
        public string Val { get; set; }
    }
    public class TranDtls
    {
        public string TaxSch { get; set; }
        public string SupTyp { get; set; }
        public string IgstOnIntra { get; set; }
        public string RegRev { get; set; }
        public string EcmGstin { get; set; }
    }
    public class DocDtls
    {
        public string Typ { get; set; }
        public string No { get; set; }
        public string Dt { get; set; }
    }
    public class SellerDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public double Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }
    public class BuyerDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Pos { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public double Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }
    public class DispDtls
    {
        public string Nm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public double Pin { get; set; }
        public string Stcd { get; set; }
    }
    public class ShipDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public double Pin { get; set; }
        public string Stcd { get; set; }
    }
    public class ValDtls
    {
        public double AssVal { get; set; }
        public double IgstVal { get; set; }
        public double CgstVal { get; set; }
        public double SgstVal { get; set; }
        public double CesVal { get; set; }
        public double StCesVal { get; set; }
        public double Discount { get; set; }
        public double OthChrg { get; set; }
        public double RndOffAmt { get; set; }
        public double TotInvVal { get; set; }
        public double TotInvValFc { get; set; }
    }
    public class ExpDtls
    {
        public string ShipBNo { get; set; }
        public string ShipBDt { get; set; }
        public string Port { get; set; }
        public string RefClm { get; set; }
        public string ForCur { get; set; }
        public string CntCode { get; set; }
        public double ExpDuty { get; set; }

    }
    public class EwbDtls
    {
        public string TransId { get; set; }
        public string TransName { get; set; }
        public string TransMode { get; set; }
        public double Distance { get; set; }
        public string TransDocNo { get; set; }
        public string TransDocDt { get; set; }
        public string VehNo { get; set; }
        public string VehType { get; set; }

    }
    public class PayDtls
    {
        public string Nm { get; set; }
        public string AccDet { get; set; }
        public string Mode { get; set; }
        public string FinInsBr { get; set; }
        public string PayTerm { get; set; }
        public string PayInstr { get; set; }
        public string CrTrn { get; set; }
        public string DirDr { get; set; }
        public double CrDay { get; set; }
        public double PaidAmt { get; set; }
        public double PaymtDue { get; set; }
    }
    public class RefDtls
    {
        public string InvRm { get; set; }
        public DocPerdDtls DocPerdDtls { get; set; } = new DocPerdDtls();
        public List<PrecDocDtls> PrecDocDtls { get; set; } = new List<PrecDocDtls>();
        public List<ContrDtls> ContrDtls { get; set; } = new List<ContrDtls>();
    }
    public class DocPerdDtls
    {
        public string InvStDt { get; set; }
        public string InvEndDt { get; set; }
    }
    public class PrecDocDtls
    {
        public string InvNo { get; set; }
        public string InvDt { get; set; }
        public string OthRefNo { get; set; }
    }
    public class ContrDtls
    {
        public string RecAdvRefr { get; set; }
        public string RecAdvDt { get; set; }
        public string TendRefr { get; set; }
        public string ContrRefr { get; set; }
        public string ExtRefr { get; set; }
        public string ProjRefr { get; set; }
        public string PORefr { get; set; }
        public string PORefDt { get; set; }
    }
}