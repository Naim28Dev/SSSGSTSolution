using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[ToolboxBitmap(typeof(System.Windows.Forms.DateTimePicker))] //Show datetimepicker icon
public class DatePicker : System.Windows.Forms.DateTimePicker
{

    /// <summary>
    /// Required designer variable.
    /// </summary>

    private System.ComponentModel.Container components = null;
    private bool realDate = true;
    private DateTimePickerFormat oldFormat;

    public DatePicker()
    {
        // This call is required by the Windows.Forms Form Designer.
        InitializeComponent();
        oldFormat = DateTimePickerFormat.Custom;
        
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
    }
    #endregion

    //Bindable true: so we can bind to the property
    //Browsable false: if browsable is set to false, the property does not appear in the property panel in the form designer
    //Browsable is by default true (a public property normally appears in the property panel)
    [Bindable(true), Browsable(false)]

    public new object Value
    {
        get
        {
            if (realDate)
            {
                return base.Value;
            }
            else
            {
                return DBNull.Value; //If not a real date, sent DBNull to the bound field
            }
        }
        set
        {
            if (Convert.IsDBNull(value))
            {
                realDate = false;
                oldFormat = Format; //Store the Format of the datetimepicker
                Format = DateTimePickerFormat.Custom;
                CustomFormat = "dd/MM/yyyy";  //With this custom format, the datetimepicker is empty
            }
            else
            {
                realDate = true;
                base.Value = Convert.ToDateTime(value);
            }
        }
    }

    protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
    {
        if (e.KeyCode == System.Windows.Forms.Keys.Delete)
        {
            Value = MinDate; //Trigger changed
            Value = DateTime.Now; //For drop down
            Value = DBNull.Value; //To DBNull
        }
        else
        {
            if (!realDate)
            {
                //If no date present and edit started, start at the current date...
                Format = oldFormat; //Restore the format for a real date
                Format = DateTimePickerFormat.Custom;
                CustomFormat = "dd/MM/yyyy";  //If you don't reset the custom format to 'null' the datetimepicker will stay empty
                realDate = true;
                Value = DateTime.Now;
            }
        }
    }

    protected override void OnCloseUp(EventArgs eventargs)
    {
        //This is the magic touch!!!!
        if (Control.MouseButtons == MouseButtons.None)
        {
            if (!realDate)
            {
                Format = oldFormat; //Restore the format for a real date
                CustomFormat = null; //If you don't reset the custom format to 'null' the datetimepicker will stay empty
                realDate = true;
                DateTime tempdate;
                tempdate = (DateTime)Value;
                Value = MinDate;
                Value = tempdate;
            }
        }
        base.OnCloseUp(eventargs);
    }

    public string ToShortDateString()
    {
        if (!realDate)
            return String.Empty;
        else
        {
            DateTime dt = (DateTime)Value;
           // return dt.ToShortDateString();
            return dt.ToShortDateString();
        }
    }
}
