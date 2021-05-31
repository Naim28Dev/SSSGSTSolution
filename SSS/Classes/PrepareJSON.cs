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
    class PrepareJSON
    {
        public static string GetGSTR1_JSON(DataSet ds, string strGSTNo, string strFP)
        {
            string strJSON = "";
            try
            {
                List<GSTR_1> objGSTR1 = new List<GSTR_1>();
                objGSTR1.Add(new GSTR_1()
                {
                    gstin = strGSTNo,
                    fp = strFP,
                    version = "GST2.3.2",
                    hash = "hash",
                    b2b = SetData_b2b(ds.Tables[0]),
                    b2ba = SetData_b2ba(ds.Tables[7]),
                    b2cl = SetData_b2cl(ds.Tables[1]),
                    b2cla = SetData_b2cla(ds.Tables[8]),
                    b2cs = SetData_b2cs(ds.Tables[2]),
                    b2csa = SetData_b2csa(ds.Tables[9]),
                    nil = SetData_NIL(new DataTable()),
                    exp = SetData_Exp(new DataTable()),
                    expa = SetData_Expa(new DataTable()),
                    cdnr = SetData_cdnr(ds.Tables[3]),
                    cdnra = SetData_cdnra(ds.Tables[10]),
                    cdnur = SetData_cdnur(ds.Tables[4]),
                    cdnura = SetData_cdnura(ds.Tables[11]),
                    doc_issue = SetData_doc_issue(ds),
                    hsn = SetData_hsn(ds.Tables[6]),
                    txpd = SetData_txpd(new DataTable()),
                    txpda = SetData_txpda(new DataTable())
                });

                strJSON = JsonConvert.SerializeObject(objGSTR1);
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return strJSON;
        }
        public static List<b2b> SetData_b2b(DataTable dt)
        {
            List<b2b> _b2b = new List<b2b>();
            try
            {
                DataTable _dt = dt.DefaultView.ToTable(true, "GSTNo");
                string strGSTNo = "", strInvoiceNo = "";
                double dTaxRate = 0, dAmt = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    strGSTNo = Convert.ToString(row["GSTNo"]);
                    DataRow[] _rows = dt.Select("GSTNo='" + strGSTNo + "' ");
                    if (_rows.Length > 0)
                    {
                        List<b2b_itm_det> _b2b_itm_det = new List<b2b_itm_det>();
                        List<b2b_itms> _b2b_itms = new List<b2b_itms>();
                        List<b2b_inv> _b2b_inv = new List<b2b_inv>();

                        DataTable __dt = _rows.CopyToDataTable().DefaultView.ToTable(true, "BillNo", "PlaceOfSupply", "BillDate", "InvoiceAmt", "ReverseCharge");
                        foreach (DataRow _row in __dt.Rows)
                        {
                            strInvoiceNo = Convert.ToString(_row["BillNo"]);
                            DataRow[] __rows = dt.Select("GSTNo='" + strGSTNo + "' and  BillNo='" + strInvoiceNo + "' ");
                            if (__rows.Length > 0)
                            {
                                foreach (DataRow ___row in __rows)
                                {
                                    dTaxRate = ConvertObjectToDouble(___row["TaxRate"]);
                                    DataRow[] ___rows = dt.Select("GSTNo='" + strGSTNo + "' and  BillNo='" + strInvoiceNo + "' and  TaxRate=" + dTaxRate + "");
                                    if (___rows.Length > 0)
                                    {
                                        foreach (DataRow dr in ___rows)
                                        {
                                            dAmt = ConvertObjectToDouble(dr["TaxableAmt"]);

                                            _b2b_itm_det.Add(new b2b_itm_det()
                                            {
                                                txval = Math.Round(dAmt, 2),
                                                rt = dTaxRate,
                                                iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                                csamt = ConvertObjectToDouble(dr["CessAmount"])
                                            });
                                        }

                                        _b2b_itms.Add(new b2b_itms()
                                        {
                                            num = ((dTaxRate * 100) + 1),
                                            itm_det = _b2b_itm_det
                                        });
                                    }
                                }
                            }

                            string strPOS = Convert.ToString(_row["PlaceOfSupply"]);
                            string[] str = strPOS.Split('-');
                            _b2b_inv.Add(new b2b_inv()
                            {
                                inum = strInvoiceNo,
                                idt = Convert.ToString(_row["BillDate"]),
                                val = ConvertObjectToDouble(_row["InvoiceAmt"]),
                                pos = str[0],
                                rchrg = Convert.ToString(_row["ReverseCharge"]),
                                //  diff_percent = 0,
                                inv_typ = "R",
                                itms = _b2b_itms
                            });
                        }

                        _b2b.Add(new b2b()
                        {
                            ctin = Convert.ToString(row["GSTNo"]),
                            inv = _b2b_inv
                        });
                    }
                }


            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _b2b;
        }

        public static List<b2ba> SetData_b2ba(DataTable dt)
        {
            List<b2ba> _b2ba = new List<b2ba>();
            try
            {
                DataTable _dt = dt.DefaultView.ToTable(true, "GSTNo");
                string strGSTNo = "", strInvoiceNo = "", strOrgInvoice = "";
                double dTaxRate = 0, dAmt = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    strGSTNo = Convert.ToString(row["GSTNo"]);
                    DataRow[] _rows = dt.Select("GSTNo='" + strGSTNo + "' ");
                    if (_rows.Length > 0)
                    {
                        List<b2b_itm_det> _b2b_itm_det = new List<b2b_itm_det>();
                        List<b2b_itms> _b2b_itms = new List<b2b_itms>();
                        List<b2ba_inv> _b2ba_inv = new List<b2ba_inv>();

                        DataTable __dt = _rows.CopyToDataTable().DefaultView.ToTable(true, "BillNo", "PlaceOfSupply", "BillDate", "InvoiceAmt", "ReverseCharge");
                        foreach (DataRow _row in __dt.Rows)
                        {
                            strInvoiceNo = Convert.ToString(_row["BillNo"]);
                            strOrgInvoice = Convert.ToString(_row["OBillNo"]);

                            DataRow[] __rows = dt.Select("GSTNo='" + strGSTNo + "' and  BillNo='" + strInvoiceNo + "'  and OBillNo='" + strOrgInvoice + "' ");
                            if (__rows.Length > 0)
                            {
                                foreach (DataRow ___row in __rows)
                                {
                                    dTaxRate = ConvertObjectToDouble(___row["TaxRate"]);
                                    DataRow[] ___rows = dt.Select("GSTNo='" + strGSTNo + "' and  BillNo='" + strInvoiceNo + "'  and OBillNo='" + strOrgInvoice + "' and  TaxRate=" + dTaxRate + "");
                                    if (___rows.Length > 0)
                                    {
                                        foreach (DataRow dr in ___rows)
                                        {
                                            dAmt = ConvertObjectToDouble(dr["TaxableAmt"]);

                                            _b2b_itm_det.Add(new b2b_itm_det()
                                            {
                                                txval = Math.Round(dAmt, 2),
                                                rt = dTaxRate,
                                                iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                                csamt = ConvertObjectToDouble(dr["CessAmount"])
                                            });
                                        }

                                        _b2b_itms.Add(new b2b_itms()
                                        {
                                            num = ((dTaxRate * 100) + 1),
                                            itm_det = _b2b_itm_det
                                        });
                                    }
                                }
                            }

                            string strPOS = Convert.ToString(_row["PlaceOfSupply"]);
                            string[] str = strPOS.Split('-');
                            _b2ba_inv.Add(new b2ba_inv()
                            {
                                oinum = strOrgInvoice,
                                oidt = Convert.ToString(_row["OBillDate"]),
                                inum = strInvoiceNo,
                                idt = Convert.ToString(_row["BillDate"]),
                                val = Math.Round(ConvertObjectToDouble(_row["InvoiceAmt"]), 2),
                                pos = str[0],
                                rchrg = Convert.ToString(_row["ReverseCharge"]),
                                //  diff_percent = 0,
                                inv_typ = "R",
                                itms = _b2b_itms
                            });
                        }

                        _b2ba.Add(new b2ba()
                        {
                            ctin = Convert.ToString(row["GSTNo"]),
                            inv = _b2ba_inv
                        });
                    }
                }


            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _b2ba;
        }

        public static List<b2cl> SetData_b2cl(DataTable dt)
        {
            List<b2cl> _b2cl = new List<b2cl>();
            try
            {
                DataTable _dt = dt.DefaultView.ToTable(true, "PlaceOfSupply");
                string strPlaceOfSupply = "", strInvoiceNo = "";
                double dTaxRate = 0, dAmt = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    strPlaceOfSupply = Convert.ToString(row["PlaceOfSupply"]);
                    DataRow[] _rows = dt.Select("PlaceOfSupply='" + strPlaceOfSupply + "' ");
                    if (_rows.Length > 0)
                    {
                        List<b2b_itm_det> _b2b_itm_det = new List<b2b_itm_det>();
                        List<b2b_itms> _b2b_itms = new List<b2b_itms>();
                        List<b2cl_inv> _b2ba_inv = new List<b2cl_inv>();

                        DataTable __dt = _rows.CopyToDataTable().DefaultView.ToTable(true, "BillNo", "PlaceOfSupply", "BillDate", "InvoiceAmt", "ReverseCharge");
                        foreach (DataRow _row in __dt.Rows)
                        {
                            strInvoiceNo = Convert.ToString(_row["BillNo"]);

                            DataRow[] __rows = dt.Select("PlaceOfSupply='" + strPlaceOfSupply + "' and  BillNo='" + strInvoiceNo + "'  ");
                            if (__rows.Length > 0)
                            {
                                foreach (DataRow ___row in __rows)
                                {
                                    dTaxRate = ConvertObjectToDouble(___row["TaxRate"]);
                                    DataRow[] ___rows = dt.Select("PlaceOfSupply='" + strPlaceOfSupply + "' and  BillNo='" + strInvoiceNo + "' and  TaxRate=" + dTaxRate + "");
                                    if (___rows.Length > 0)
                                    {
                                        foreach (DataRow dr in ___rows)
                                        {
                                            dAmt = ConvertObjectToDouble(dr["TaxableAmt"]);

                                            _b2b_itm_det.Add(new b2b_itm_det()
                                            {
                                                txval = Math.Round(dAmt, 2),
                                                rt = dTaxRate,
                                                iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                                csamt = ConvertObjectToDouble(dr["CessAmount"])
                                            });
                                        }

                                        _b2b_itms.Add(new b2b_itms()
                                        {
                                            num = ((dTaxRate * 100) + 1),
                                            itm_det = _b2b_itm_det
                                        });
                                    }
                                }
                            }


                            _b2ba_inv.Add(new b2cl_inv()
                            {
                                inum = strInvoiceNo,
                                idt = Convert.ToString(_row["BillDate"]),
                                val = ConvertObjectToDouble(_row["InvoiceAmt"]),
                                inv_typ = "R",
                                itms = _b2b_itms
                            });
                        }

                        string[] str = strPlaceOfSupply.Split('-');
                        _b2cl.Add(new b2cl()
                        {
                            pos = str[0],
                            inv = _b2ba_inv
                        });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _b2cl;
        }

        public static List<b2cla> SetData_b2cla(DataTable dt)
        {
            List<b2cla> _b2cla = new List<b2cla>();
            try
            {
                DataTable _dt = dt.DefaultView.ToTable(true, "PlaceOfSupply");
                string strPlaceOfSupply = "", strInvoiceNo = "", strOrgInvoice = "";
                double dTaxRate = 0, dAmt = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    strPlaceOfSupply = Convert.ToString(row["PlaceOfSupply"]);
                    DataRow[] _rows = dt.Select("PlaceOfSupply='" + strPlaceOfSupply + "' ");
                    if (_rows.Length > 0)
                    {
                        List<b2b_itm_det> _b2b_itm_det = new List<b2b_itm_det>();
                        List<b2b_itms> _b2b_itms = new List<b2b_itms>();
                        List<b2cla_inv> _b2ba_inv = new List<b2cla_inv>();

                        DataTable __dt = _rows.CopyToDataTable().DefaultView.ToTable(true, "BillNo", "PlaceOfSupply", "BillDate", "InvoiceAmt", "ReverseCharge");
                        foreach (DataRow _row in __dt.Rows)
                        {
                            strInvoiceNo = Convert.ToString(_row["BillNo"]);
                            strOrgInvoice = Convert.ToString(_row["OBillNo"]);
                            DataRow[] __rows = dt.Select("PlaceOfSupply='" + strPlaceOfSupply + "' and  BillNo='" + strInvoiceNo + "'  and OBillNo='" + strOrgInvoice + "'  ");
                            if (__rows.Length > 0)
                            {
                                foreach (DataRow ___row in __rows)
                                {
                                    dTaxRate = ConvertObjectToDouble(___row["TaxRate"]);
                                    DataRow[] ___rows = dt.Select("PlaceOfSupply='" + strPlaceOfSupply + "' and  BillNo='" + strInvoiceNo + "' and  TaxRate=" + dTaxRate + "   and OBillNo='" + strOrgInvoice + "' ");
                                    if (___rows.Length > 0)
                                    {
                                        foreach (DataRow dr in ___rows)
                                        {
                                            dAmt = ConvertObjectToDouble(dr["TaxableAmt"]);

                                            _b2b_itm_det.Add(new b2b_itm_det()
                                            {
                                                txval = Math.Round(dAmt, 2),
                                                rt = dTaxRate,
                                                iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                                csamt = ConvertObjectToDouble(dr["CessAmount"])
                                            });
                                        }

                                        _b2b_itms.Add(new b2b_itms()
                                        {
                                            num = ((dTaxRate * 100) + 1),
                                            itm_det = _b2b_itm_det
                                        });
                                    }
                                }
                            }


                            _b2ba_inv.Add(new b2cla_inv()
                            {
                                oinum = strOrgInvoice,
                                oidt = Convert.ToString(_row["OBillDate"]),
                                inum = strInvoiceNo,
                                idt = Convert.ToString(_row["BillDate"]),
                                val = ConvertObjectToDouble(_row["InvoiceAmt"]),
                                inv_typ = "R",
                                itms = _b2b_itms
                            });
                        }

                        string[] str = strPlaceOfSupply.Split('-');
                        _b2cla.Add(new b2cla()
                        {
                            pos = str[0],
                            inv = _b2ba_inv
                        });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _b2cla;
        }

        public static List<b2cs> SetData_b2cs(DataTable dt)
        {
            List<b2cs> _b2cs = new List<b2cs>();
            try
            {
                double dTaxRate = 0, dAmt = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string strPOS = Convert.ToString(row["PlaceOfSupply"]);
                    string[] str = strPOS.Split('-');
                    dAmt = ConvertObjectToDouble(row["TaxableAmt"]);
                    dTaxRate = ConvertObjectToDouble(row["TaxRate"]);
                    _b2cs.Add(new b2cs()
                    {
                        sply_ty = "INTER",
                        rt = dTaxRate,
                        typ = Convert.ToString(row["EcommType"]),
                        pos = str[0],
                        txval = Math.Round(dAmt, 2),
                        iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                        csamt = ConvertObjectToDouble(row["CessAmount"])
                    });

                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _b2cs;
        }

        public static List<b2csa> SetData_b2csa(DataTable dt)
        {
            List<b2csa> _b2csa = new List<b2csa>();
            try
            {
                string strPlaceofSupply = "", strFinYear = "", strEcommType = "";
                DataTable _dt = dt.DefaultView.ToTable(true, "FYear", "PlaceOfSupply", "EcommType");
                double dTaxRate = 0, dAmt = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    List<b2csa_itms> _b2csa_itms = new List<b2csa_itms>();
                    strFinYear = Convert.ToString(row["FYear"]);
                    strPlaceofSupply = Convert.ToString(row["PlaceOfSupply"]);
                    strEcommType = Convert.ToString(row["EcommType"]);

                    DataRow[] _rows = dt.Select("PlaceOfSupply='" + strPlaceofSupply + "' and FYear='" + strFinYear + "' ");
                    if (_rows.Length > 0)
                    {
                        foreach (DataRow __row in _rows)
                        {
                            dTaxRate = ConvertObjectToDouble(__row["TaxRate"]);
                            dAmt = ConvertObjectToDouble(__row["TaxableAmt"]);
                            _b2csa_itms.Add(new b2csa_itms()
                            {
                                rt = dTaxRate,
                                txval = Math.Round(dAmt, 2),
                                iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                csamt = ConvertObjectToDouble(__row["CessAmount"])
                            });
                        }
                    }

                    string[] str = strPlaceofSupply.Split('-');
                    _b2csa.Add(new b2csa()
                    {
                        omon = strFinYear,
                        sply_ty = "INTER",
                        typ = Convert.ToString(row["EcommType"]),
                        pos = str[0],
                        itms = _b2csa_itms
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _b2csa;
        }
        public static List<nil> SetData_NIL(DataTable dt)
        {
            List<nil> _nil = new List<nil>();
            List<nil_inv> _nil_inv = new List<nil_inv>();
            try
            {
                _nil.Add(new nil()
                {
                    inv = _nil_inv
                });

            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _nil;
        }

        public static List<exp> SetData_Exp(DataTable dt)
        {
            List<exp> _exp = new List<exp>();
            List<exp_inv> _exp_inv = new List<exp_inv>();
            try
            {
                _exp.Add(new exp()
                {
                    inv = _exp_inv
                });
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _exp;
        }

        public static List<expa> SetData_Expa(DataTable dt)
        {
            List<expa> _exp = new List<expa>();
            List<expa_inv> _exp_inv = new List<expa_inv>();
            try
            {
                _exp.Add(new expa()
                {
                    inv = _exp_inv
                });
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _exp;
        }

        public static List<cdnr> SetData_cdnr(DataTable dt)
        {
            List<cdnr> _cdnr = new List<cdnr>();
            try
            {
                DataTable _dt = dt.DefaultView.ToTable(true, "GSTNo");
                string strGSTNo = "", strInvoiceNo = "", strOrgInvoice = "";
                double dTaxRate = 0, dAmt = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    strGSTNo = Convert.ToString(row["GSTNo"]);
                    DataRow[] _rows = dt.Select("GSTNo='" + strGSTNo + "' ");
                    if (_rows.Length > 0)
                    {
                        List<b2b_itm_det> _b2b_itm_det = new List<b2b_itm_det>();
                        List<b2b_itms> _b2b_itms = new List<b2b_itms>();
                        List<cdnr_nt> _cdnr_nt = new List<cdnr_nt>();

                        DataTable __dt = _rows.CopyToDataTable().DefaultView.ToTable(true, "BillNo", "BillDate", "SaleBillNo", "SaleBillDate", "DocType", "NetAmt", "PreGST", "PlaceOfSupply");
                        foreach (DataRow _row in __dt.Rows)
                        {
                            strInvoiceNo = Convert.ToString(_row["BillNo"]);
                            strOrgInvoice = Convert.ToString(_row["SaleBillNo"]);

                            DataRow[] __rows = dt.Select("GSTNo='" + strGSTNo + "' and  BillNo='" + strInvoiceNo + "'  and SaleBillNo='" + strOrgInvoice + "' ");
                            if (__rows.Length > 0)
                            {
                                foreach (DataRow ___row in __rows)
                                {
                                    dTaxRate = ConvertObjectToDouble(___row["TaxRate"]);
                                    DataRow[] ___rows = dt.Select("GSTNo='" + strGSTNo + "' and  BillNo='" + strInvoiceNo + "'  and SaleBillNo='" + strOrgInvoice + "' and  TaxRate=" + dTaxRate + "");
                                    if (___rows.Length > 0)
                                    {
                                        foreach (DataRow dr in ___rows)
                                        {
                                            dAmt = ConvertObjectToDouble(dr["TaxableAmt"]);

                                            _b2b_itm_det.Add(new b2b_itm_det()
                                            {
                                                txval = Math.Round(dAmt, 2),
                                                rt = dTaxRate,
                                                iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                                csamt = 0,// ConvertObjectToDouble(dr["CessAmount"])
                                            });
                                        }

                                        _b2b_itms.Add(new b2b_itms()
                                        {
                                            num = ((dTaxRate * 100) + 1),
                                            itm_det = _b2b_itm_det
                                        });
                                    }
                                }
                            }

                            string strPOS = Convert.ToString(_row["PlaceOfSupply"]);
                            string[] str = strPOS.Split('-');
                            _cdnr_nt.Add(new cdnr_nt()
                            {
                                nt_num = strInvoiceNo,
                                nt_dt = Convert.ToString(_row["BillDate"]),
                                inum = strOrgInvoice,
                                ntty = Convert.ToString(_row["DocType"]),
                                idt = Convert.ToString(_row["SaleBillDate"]),
                                val = ConvertObjectToDouble(_row["NetAmt"]),
                                p_gst = Convert.ToString(_row["PreGST"]),
                                itms = _b2b_itms
                            });
                        }

                        _cdnr.Add(new cdnr()
                        {
                            ctin = strGSTNo,
                            nt = _cdnr_nt
                        });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _cdnr;
        }

        public static List<cdnra> SetData_cdnra(DataTable dt)
        {
            List<cdnra> _cdnr = new List<cdnra>();
            try
            {
                DataTable _dt = dt.DefaultView.ToTable(true, "GSTNo");
                string strGSTNo = "", strInvoiceNo = "", strSaleInvoice = "", strOrgInvoice = "";
                double dTaxRate = 0, dAmt = 0;
                foreach (DataRow row in _dt.Rows)
                {
                    strGSTNo = Convert.ToString(row["GSTNo"]);
                    DataRow[] _rows = dt.Select("GSTNo='" + strGSTNo + "' ");
                    if (_rows.Length > 0)
                    {
                        List<b2b_itm_det> _b2b_itm_det = new List<b2b_itm_det>();
                        List<b2b_itms> _b2b_itms = new List<b2b_itms>();
                        List<cdnra_nt> _cdnr_nt = new List<cdnra_nt>();

                        DataTable __dt = _rows.CopyToDataTable().DefaultView.ToTable(true, "ORBillNo", "ORDate", "BillNo", "BillDate", "SaleBillNo", "SaleBillDate", "DocType", "NetAmt", "PreGST");
                        foreach (DataRow _row in __dt.Rows)
                        {
                            strInvoiceNo = Convert.ToString(_row["BillNo"]);
                            strSaleInvoice = Convert.ToString(_row["SaleBillNo"]);
                            strOrgInvoice = Convert.ToString(_row["ORBillNo"]);

                            DataRow[] __rows = dt.Select("GSTNo='" + strGSTNo + "' and  BillNo='" + strInvoiceNo + "'  and SaleBillNo='" + strSaleInvoice + "''  and ORBillNo='" + strOrgInvoice + "' ");
                            if (__rows.Length > 0)
                            {
                                foreach (DataRow ___row in __rows)
                                {
                                    dTaxRate = ConvertObjectToDouble(___row["TaxRate"]);
                                    DataRow[] ___rows = dt.Select("GSTNo='" + strGSTNo + "' and  BillNo='" + strInvoiceNo + "'  and SaleBillNo='" + strSaleInvoice + "' '  and ORBillNo='" + strOrgInvoice + "' and  TaxRate=" + dTaxRate + "");
                                    if (___rows.Length > 0)
                                    {
                                        foreach (DataRow dr in ___rows)
                                        {
                                            dAmt = ConvertObjectToDouble(dr["TaxableAmt"]);

                                            _b2b_itm_det.Add(new b2b_itm_det()
                                            {
                                                txval = Math.Round(dAmt, 2),
                                                rt = dTaxRate,
                                                iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                                csamt = ConvertObjectToDouble(dr["CessAmount"])
                                            });
                                        }

                                        _b2b_itms.Add(new b2b_itms()
                                        {
                                            num = ((dTaxRate * 100) + 1),
                                            itm_det = _b2b_itm_det
                                        });
                                    }
                                }
                            }

                            string strPOS = Convert.ToString(_row["PlaceOfSupply"]);
                            string[] str = strPOS.Split('-');
                            _cdnr_nt.Add(new cdnra_nt()
                            {
                                ont_num = strOrgInvoice,
                                ont_dt = Convert.ToString(_row["ORDate"]),
                                nt_num = strInvoiceNo,
                                nt_dt = Convert.ToString(_row["BillDate"]),
                                inum = strSaleInvoice,
                                ntty = Convert.ToString(_row["DocType"]),
                                idt = Convert.ToString(_row["SaleBillDate"]),
                                val = ConvertObjectToDouble(_row["NetAmt"]),
                                p_gst = Convert.ToString(_row["PreGST"]),
                                itms = _b2b_itms
                            });
                        }

                        _cdnr.Add(new cdnra()
                        {
                            ctin = strGSTNo,
                            nt = _cdnr_nt
                        });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _cdnr;
        }

        public static List<at> SetData_at(DataTable dt)
        {
            List<at> _at = new List<at>();
            List<at_itms> _at_itms = new List<at_itms>();
            try
            {

                //_at.Add(new at()
                //{
                //    itms = _at_itms
                //});
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _at;
        }

        public static List<ata> SetData_ata(DataTable dt)
        {
            List<ata> _ata = new List<ata>();
            List<at_itms> _ata_itms = new List<at_itms>();
            try
            {

                //_ata.Add(new ata()
                //{
                //    itms = _ata_itms
                //});
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _ata;
        }

        public static List<cdnur> SetData_cdnur(DataTable dt)
        {
            List<cdnur> _cdnur = new List<cdnur>();
            try
            {
                string strInvoiceNo = "", strOrgInvoice = "";
                double dTaxRate = 0, dAmt = 0;

                DataTable __dt = dt.DefaultView.ToTable(true, "URType", "BillNo", "BillDate", "SaleBillNo", "SaleBillDate", "DocType", "NetAmt", "PreGST");
                foreach (DataRow _row in __dt.Rows)
                {

                    List<b2b_itm_det> _b2b_itm_det = new List<b2b_itm_det>();
                    List<b2b_itms> _b2b_itms = new List<b2b_itms>();

                    strInvoiceNo = Convert.ToString(_row["BillNo"]);
                    strOrgInvoice = Convert.ToString(_row["SaleBillNo"]);

                    DataRow[] __rows = dt.Select("BillNo='" + strInvoiceNo + "'  and SaleBillNo='" + strOrgInvoice + "' ");
                    if (__rows.Length > 0)
                    {
                        foreach (DataRow ___row in __rows)
                        {
                            dTaxRate = ConvertObjectToDouble(___row["TaxRate"]);
                            DataRow[] ___rows = dt.Select("BillNo=" + strInvoiceNo + "  and SaleBillNo='" + strOrgInvoice + "' and  TaxRate=" + dTaxRate + "");
                            if (___rows.Length > 0)
                            {
                                foreach (DataRow dr in ___rows)
                                {
                                    dAmt = ConvertObjectToDouble(dr["TaxableAmt"]);

                                    _b2b_itm_det.Add(new b2b_itm_det()
                                    {
                                        txval = Math.Round(dAmt, 2),
                                        rt = dTaxRate,
                                        iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                        csamt = ConvertObjectToDouble(dr["CessAmount"])
                                    });
                                }

                                _b2b_itms.Add(new b2b_itms()
                                {
                                    num = ((dTaxRate * 100) + 1),
                                    itm_det = _b2b_itm_det
                                });
                            }
                        }
                    }


                    _cdnur.Add(new cdnur()
                    {
                        nt_num = strInvoiceNo,
                        nt_dt = Convert.ToString(_row["BillDate"]),
                        inum = strOrgInvoice,
                        ntty = Convert.ToString(_row["DocType"]),
                        idt = Convert.ToString(_row["SaleBillDate"]),
                        val = ConvertObjectToDouble(_row["NetAmt"]),
                        p_gst = Convert.ToString(_row["PreGST"]),
                        typ = Convert.ToString(_row["URType"]),
                        itms = _b2b_itms
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _cdnur;
        }

        public static List<cdnura> SetData_cdnura(DataTable dt)
        {
            List<cdnura> _cdnura = new List<cdnura>();
            try
            {
                string strInvoiceNo = "", strOrgInvoice = "";
                double dTaxRate = 0, dAmt = 0;

                DataTable __dt = dt.DefaultView.ToTable(true, "URType", "ORBillNo", "ORDate", "BillNo", "BillDate", "SaleBillNo", "SaleBillDate", "DocType", "NetAmt", "PreGST");
                foreach (DataRow _row in __dt.Rows)
                {

                    List<b2b_itm_det> _b2b_itm_det = new List<b2b_itm_det>();
                    List<b2b_itms> _b2b_itms = new List<b2b_itms>();

                    strInvoiceNo = Convert.ToString(_row["BillNo"]);
                    strOrgInvoice = Convert.ToString(_row["SaleBillNo"]);

                    DataRow[] __rows = dt.Select("BillNo='" + strInvoiceNo + "'  and SaleBillNo='" + strOrgInvoice + "' ");
                    if (__rows.Length > 0)
                    {
                        foreach (DataRow ___row in __rows)
                        {
                            dTaxRate = ConvertObjectToDouble(___row["TaxRate"]);
                            DataRow[] ___rows = dt.Select("BillNo=" + strInvoiceNo + "  and SaleBillNo='" + strOrgInvoice + "' and  TaxRate=" + dTaxRate + "");
                            if (___rows.Length > 0)
                            {
                                foreach (DataRow dr in ___rows)
                                {
                                    dAmt = ConvertObjectToDouble(dr["TaxableAmt"]);

                                    _b2b_itm_det.Add(new b2b_itm_det()
                                    {
                                        txval = Math.Round(dAmt, 2),
                                        rt = dTaxRate,
                                        iamt = Math.Round(((dTaxRate * dAmt) / 100), 2),
                                        csamt = ConvertObjectToDouble(dr["CessAmount"])
                                    });
                                }

                                _b2b_itms.Add(new b2b_itms()
                                {
                                    num = ((dTaxRate * 100) + 1),
                                    itm_det = _b2b_itm_det
                                });
                            }
                        }
                    }

                    _cdnura.Add(new cdnura()
                    {

                        ont_num = Convert.ToString(_row["ORBillNo"]),
                        ont_dt = Convert.ToString(_row["ORDate"]),
                        nt_num = strInvoiceNo,
                        nt_dt = Convert.ToString(_row["BillDate"]),
                        inum = strOrgInvoice,
                        ntty = Convert.ToString(_row["DocType"]),
                        idt = Convert.ToString(_row["SaleBillDate"]),
                        val = ConvertObjectToDouble(_row["NetAmt"]),
                        p_gst = Convert.ToString(_row["PreGST"]),
                        typ = Convert.ToString(_row["URType"]),
                        itms = _b2b_itms
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _cdnura;
        }


        public static List<doc_issue> SetData_doc_issue(DataSet ds)
        {
            List<doc_issue> _doc_issue = new List<doc_issue>();
            List<doc_det> _doc_det = new List<doc_det>();
            List<doc_issue_docs> _doc_issue_docs = new List<doc_issue_docs>();

            try
            {
                DataTable dt = ds.Tables[12];
                int _count = 1;
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    _doc_issue_docs.Add(new doc_issue_docs()
                    {
                        num = _count,
                        from = row["BCode"] + "" + row["MinBillNo"],
                        to = row["BCode"] + "" + row["MaxBillNo"],
                        totnum = ConvertObjectToDouble(row["TotalBill"]),
                        cancel = ConvertObjectToDouble(row["MissNum"]),
                        net_issue = ConvertObjectToDouble(row["NetBill"])
                    });
                    _count++;
                }
                dt = ds.Tables[13];
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    _doc_issue_docs.Add(new doc_issue_docs()
                    {
                        num = _count,
                        from = row["BCode"] + "" + row["MinBillNo"],
                        to = row["BCode"] + "" + row["MaxBillNo"],
                        totnum = ConvertObjectToDouble(row["TotalBill"]),
                        cancel = ConvertObjectToDouble(row["MissNum"]),
                        net_issue = ConvertObjectToDouble(row["NetBill"])
                    });
                }

                _doc_det.Add(new doc_det()
                {
                    doc_num = 1,
                    doc_typ = "Invoices for outward supply",
                    docs = _doc_issue_docs
                });

                _doc_issue.Add(new doc_issue()
                {
                    doc_det = _doc_det
                });

            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _doc_issue;
        }

        public static List<hsn> SetData_hsn(DataTable dt)
        {
            List<hsn> _hsn = new List<hsn>();
            List<hsn_data> _hsn_data = new List<hsn_data>();
            try
            {
                int _count = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    string strUQC = Convert.ToString(dr["UnitName"]);
                    string[] str = strUQC.Split('-');
                    _hsn_data.Add(new hsn_data()
                    {
                        num = _count,
                        hsn_sc = Convert.ToString(dr["HSNCode"]),
                        desc = Convert.ToString(dr["ItemName"]),
                        uqc = str[0],
                        qty = Math.Round(ConvertObjectToDouble(dr["Qty"]), 2),
                        val = ConvertObjectToDouble(dr["TotalValue"]),
                        txval = Math.Round(ConvertObjectToDouble(dr["TaxableAmt"]), 2),
                        iamt = Math.Round(ConvertObjectToDouble(dr["IGSTAmt"]), 2),
                        samt = Math.Round(ConvertObjectToDouble(dr["SGSTAmt"]), 2),
                        camt = Math.Round(ConvertObjectToDouble(dr["CGSTAmt"]), 2),
                        csamt = ConvertObjectToDouble(dr["CessAmt"])
                    });
                    _count++;
                }

                _hsn.Add(new hsn()
                {
                    data = _hsn_data
                });

            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _hsn;
        }

        public static List<txpd> SetData_txpd(DataTable dt)
        {
            List<txpd> _txpd = new List<txpd>();
            List<at_itms> _at_itms = new List<at_itms>();
            try
            {

                //_txpd.Add(new txpd()
                //{
                //    itms = _at_itms
                //});
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _txpd;
        }

        public static List<txpda> SetData_txpda(DataTable dt)
        {
            List<txpda> _txpda = new List<txpda>();
            List<at_itms> _at_itms = new List<at_itms>();
            try
            {

                //_txpd.Add(new txpd()
                //{
                //    itms = _at_itms
                //});
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return _txpda;
        }


        public static double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            try
            {
                if (Convert.ToString(objValue) != "")
                    dValue = Convert.ToDouble(objValue);
            }
            catch
            {
            }
            return dValue;
        }


        public static string GetGSTR3B_JSON(DataSet ds, string strGSTNo, string strFP)
        {
            string strJSON = "";
            try
            {
                DataTable dt = ds.Tables[3];

                double dAmt = 0, dIGST = 0, dCGST = 0;
                double dPIGST = 0, dPCGST = 0, dRIGST = 0, dRCGST = 0;
                osup_det _osup_det = new osup_det();
                if (dt.Rows.Count > 0)
                {
                    dAmt = Math.Round(ConvertObjectToDouble(dt.Rows[0]["Amount"]), 2);
                    dIGST = Math.Round(ConvertObjectToDouble(dt.Rows[0]["IGST"]), 2);
                    dCGST = Math.Round(ConvertObjectToDouble(dt.Rows[0]["CGST"]), 2);
                    {
                        _osup_det.txval = dAmt;
                        _osup_det.iamt = dIGST;
                        _osup_det.camt = dCGST;
                        _osup_det.samt = dCGST;
                        _osup_det.csamt = 0;
                    }
                }

                osup_zero _osup_zero = new osup_zero();               
                _osup_zero.txval = _osup_zero.iamt = _osup_zero.camt = _osup_zero.samt = _osup_zero.csamt = 0;
                

                osup_nil_exmp _osup_nil_exmp = new osup_nil_exmp();
                _osup_nil_exmp.txval = _osup_nil_exmp.iamt = _osup_nil_exmp.camt = _osup_nil_exmp.samt = _osup_nil_exmp.csamt = 0;

                osup_nongst _osup_nongst = new osup_nongst();
                _osup_nongst.txval = _osup_nongst.iamt = _osup_nongst.camt = _osup_nongst.samt = _osup_nongst.csamt = 0;

                isup_rev _isup_rev = new isup_rev();
                dt = ds.Tables[2];
                if (dt.Rows.Count > 0)
                {
                    dAmt = Math.Round(ConvertObjectToDouble(dt.Rows[0]["Amount"]), 2);
                    dIGST = Math.Round(ConvertObjectToDouble(dt.Rows[0]["IGST"]), 2);
                    dCGST = Math.Round(ConvertObjectToDouble(dt.Rows[0]["CGST"]), 2);
                    _isup_rev.txval = dAmt;
                    _isup_rev.iamt = dIGST;
                    _isup_rev.camt = dCGST;
                    _isup_rev.samt = dCGST;
                    _isup_rev.csamt = 0;
                }

                sup_details _sup_details = new sup_details();              
                {
                    _sup_details.osup_det = _osup_det;
                    _sup_details.osup_zero = _osup_zero;
                    _sup_details.osup_nil_exmp = _osup_nil_exmp;
                    _sup_details.isup_rev = _isup_rev;
                    _sup_details.osup_nongst = _osup_nongst;
                }

                List<itc_avl> _itc_avl = new List<itc_avl>();
                _itc_avl.Add(new itc_avl()
                {
                    ty = "IMPG",
                    iamt = 0,
                    camt = 0,
                    samt = 0,
                    csamt = 0
                });
                _itc_avl.Add(new itc_avl()
                {
                    ty = "IMPS", iamt = 0, camt = 0, samt = 0, csamt = 0
                });

                _itc_avl.Add(new itc_avl()
                {
                    ty = "ISRC",
                    iamt = 0,
                    camt = 0,
                    samt = 0,
                    csamt = 0
                });

                _itc_avl.Add(new itc_avl()
                {
                    ty = "ISD",
                    iamt = 0,
                    camt = 0,
                    samt = 0,
                    csamt = 0
                });

                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    dPIGST = Math.Round(ConvertObjectToDouble(dt.Rows[0]["IGST"]), 2);
                    dPCGST = Math.Round(ConvertObjectToDouble(dt.Rows[0]["CGST"]), 2);

                    _itc_avl.Add(new itc_avl()
                    {
                        ty = "OTH",
                        iamt = dPIGST,
                        camt = dPCGST,
                        samt = dPCGST,
                        csamt = 0
                    });
                }

                List<itc_rev> _itc_rev = new List<itc_rev>();
                _itc_rev.Add(new itc_rev()
                {
                    ty = "RUL",
                    iamt = 0,
                    camt = 0,
                    samt = 0,
                    csamt = 0
                });

                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    dRIGST = Math.Round(ConvertObjectToDouble(dt.Rows[0]["IGST"]), 2);
                    dRCGST = Math.Round(ConvertObjectToDouble(dt.Rows[0]["CGST"]), 2);

                    _itc_rev.Add(new itc_rev()
                    {
                        ty = "OTH",
                        iamt = dRIGST,
                        camt = dRCGST,
                        samt = dRCGST,
                        csamt = 0
                    });
                }

                List<itc_net> _itc_net = new List<itc_net>();
                _itc_net.Add(new itc_net()
                {
                    iamt = Math.Round((dPIGST - dRIGST), 2),
                    camt = Math.Round((dPCGST - dRCGST), 2),
                    samt = Math.Round((dPCGST - dRCGST), 2),
                    csamt = 0
                });

                List<itc_inelg> _itc_inelg = new List<itc_inelg>();
                _itc_inelg.Add(new itc_inelg()
                {
                    ty = "RUL",
                    iamt = 0,
                    camt = 0,
                    samt = 0,
                    csamt = 0
                });

                _itc_inelg.Add(new itc_inelg()
                {
                    ty = "OTH",
                    iamt = 0,
                    camt = 0,
                    samt = 0,
                    csamt = 0
                });

                itc_elg _itc_elg = new itc_elg();               
                {
                    _itc_elg.itc_avl = _itc_avl;
                    _itc_elg.itc_rev = _itc_rev;
                    _itc_elg.itc_net = _itc_net;
                    _itc_elg.itc_inelg = _itc_inelg;
                }

                List<isup_details> _isup_details = new List<isup_details>();
                _isup_details.Add(new isup_details()
                {
                    ty = "GST",
                    inter = 0,
                    intra = 0
                });
                _isup_details.Add(new isup_details()
                {
                    ty = "NONGST",
                    inter = 0,
                    intra = 0
                });

                inward_sup _inward_sup = new inward_sup();
                {
                    _inward_sup.isup_details = _isup_details;
                }

                List<intr_details> _intr_details = new List<intr_details>();
                _intr_details .Add(new intr_details()
                {
                    iamt = 0,
                    camt = 0,
                    samt = 0,
                    csamt = 0
                });
                List<ltfee_details> _ltfee_details = new List<ltfee_details>();

                intr_ltfee _intr_ltfee = new intr_ltfee();
                {
                    _intr_ltfee.intr_details = _intr_details;
                    _intr_ltfee.ltfee_details = _ltfee_details;
                }

                inter_sup _inter_sup = new inter_sup();
                _inter_sup.unreg_details = new List<unreg_details>();
                _inter_sup.comp_details = new List<comp_details>();
                _inter_sup.uin_details = new List<uin_details>();

                GSTR_3B _GSTR_3B = new GSTR_3B();              
                {
                    _GSTR_3B.gstin = strGSTNo;
                    _GSTR_3B.ret_period = strFP;
                    _GSTR_3B.sup_details = _sup_details;
                    _GSTR_3B.itc_elg = _itc_elg;
                    _GSTR_3B.inward_sup = _inward_sup;
                    _GSTR_3B.intr_ltfee = _intr_ltfee;
                    _GSTR_3B.inter_sup = _inter_sup;
                }

                 strJSON = JsonConvert.SerializeObject(_GSTR_3B);
            }
            catch (Exception ex) { MessageBox.Show("Sorry " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return strJSON;
        }

    }

}
