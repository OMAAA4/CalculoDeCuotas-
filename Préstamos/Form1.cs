using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Préstamos
{
    public delegate void ErrorEnDatos(Control e,string Mesaje);    

    public partial class Form1 : Form
    {
        double TMonto, TTasa, TSeguro, TCuota, InteresAcumulado = 0.0, SeguroAcomulado = 0.0;
        int TPlazo, TiempoDeVida = 0;
        bool PagoExtra = false;
        DataTable dtextra = new DataTable();
        BindingSource bs = new BindingSource();

        public Form1()
        {
            InitializeComponent();
        }

        Func<double, double, int, double, double> CalcularCuota 
            = ( Monto,  tasa,  Plazo, Seguro) 
            => { return Math.Round(((Monto * ((tasa / 100) / 12)) / (1 - Math.Pow(1 + ((tasa / 100) / 12), -Plazo))) + Seguro, 2); };

        Func<double, double, double> CalcularInteres 
            = (Saldo, tasa) => { return Math.Round((Saldo * ((tasa / 100) / 12)), 2); };

        Func<double, double, int, double> CalcularInteresPorDias
            = (Saldo, Tasa, Dias) => { return Math.Round((Saldo * (Tasa / 100)) * (Dias / 360.0),2); };

        void ErroresCampos(Control e, string Mesaje)
        {
            errorProvider1.SetError(e, Mesaje);
        }

        void CamposModificados(Control e, string Mesaje)
        {
            errorProvider2.SetError(e, Mesaje);
            errorProvider2.Icon = Properties.Resources.ad;
        }
        void CuotaModificada(Control e, string Mesaje)
        {
            errorProvider3.SetError(e, Mesaje);
            errorProvider3.Icon = Properties.Resources.ad;
        }

        private void aGREGARCUOTAFIJAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            txtCuota.Enabled = false;
            bool Monto, Tasa, Plazo, Seguro;
            Monto = double.TryParse(txtMonto.Text.Replace("$",""), out TMonto);
            Tasa = double.TryParse(txtTasa.Text.Replace("%",""), out TTasa);
            Plazo = int.TryParse(txtPlazo.Text, out TPlazo);
            Seguro = double.TryParse(txtSeguro.Text.Replace("$", ""), out TSeguro);

            ErrorEnDatos error = new ErrorEnDatos(ErroresCampos);

            if (!Monto)
            {
                error(txtMonto,"Error Con el monto");
            }

            if (!Tasa)
            {
                error(txtTasa, "Error con la tasa");
            }

            if (!Plazo)
            {
                error(txtPlazo, "Error con el Plazo");
            }
            if (!Seguro)
            {
                error(txtSeguro, "Error con el Seguro");
            }
            
            if(Monto && Tasa && Plazo && Seguro)
            {
                TCuota = CalcularCuota(TMonto,TTasa,TPlazo,TSeguro);
                txtCuota.Text = string.Concat("$",TCuota.ToString());
            }
        }

        private void rEALIZARCALCULOf5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TablaAmortización();
        }

        private void txtMonto_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCuota.Text))
            {
                ErrorEnDatos error = new ErrorEnDatos(CamposModificados);
                error(txtMonto, "Campo modificado");
            }
        }

        private void txtTasa_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCuota.Text))
            {
                ErrorEnDatos error = new ErrorEnDatos(CamposModificados);
                error(txtTasa, "Campo modificado");
            }
        }

        private void txtPlazo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCuota.Text))
            {
                ErrorEnDatos error = new ErrorEnDatos(CamposModificados);
                error(txtPlazo, "Campo modificado");
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ErrorEnDatos error = new ErrorEnDatos(CuotaModificada);
            error(dataGridView1, "Se modificaron pagos extras");
        }

        private void txtMonto_Leave(object sender, EventArgs e)
        {
            if (!txtMonto.Text.StartsWith("$"))
            {
                string formato = string.Concat("$", txtMonto.Text);
                txtMonto.Text = formato;
            }
        }

        private void txtTasa_Leave(object sender, EventArgs e)
        {
            if (!txtTasa.Text.EndsWith("%"))
            {
                string formato = string.Concat(txtTasa.Text, "%");
                txtTasa.Text = formato;
            }
        }

        private void txtSeguro_Leave(object sender, EventArgs e)
        {
            if (!txtSeguro.Text.StartsWith("$"))
            {
                string formato = string.Concat("$", txtSeguro.Text);
                txtSeguro.Text = formato;
            }
        }

        private void txtSeguro_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCuota.Text))
            {
                ErrorEnDatos error = new ErrorEnDatos(CamposModificados);
                error(txtSeguro, "Campo modificado");
            }
        }

        private void txtCuota_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCuota.Text))
            {
                ErrorEnDatos error = new ErrorEnDatos(CuotaModificada);
                error(dataGridView1, "Cargue la tabla");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            groupBox3.Visible = false;

            dtextra.Columns.Add("Mes", typeof(int));
            dtextra.Columns.Add("Dia", typeof(int));
            dtextra.Columns.Add("Monto", typeof(double));

            bs.DataSource = dtextra;
            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.DataSource = bs;
        }

        private void cALCULARCUOTAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (groupBox3.Visible)
            {
                groupBox3.Visible = false;
                PagoExtra = false;
            }
            else
            {
                groupBox3.Visible = true;
                PagoExtra = true;
            }
        }
        private void aGREGARSEGURODatoPersonalizadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtSeguro.Enabled = true;
        }


        void TablaAmortización()
        {
            errorProvider1.Clear();
            errorProvider3.Clear();
            ErrorEnDatos error = new ErrorEnDatos(ErroresCampos);
            DataTable dt = new DataTable();
            dt.Columns.Add("Mes", typeof(string));
            dt.Columns.Add("Cuota", typeof(double));
            dt.Columns.Add("Interes", typeof(double));
            dt.Columns.Add("Seguro", typeof(double));
            dt.Columns.Add("Saldo", typeof(double));
            dt.Columns.Add("Capital", typeof(double));

            DataView dv = new DataView();
            DataTable CuotasExtras = new DataTable();
            if (PagoExtra)
            {
                dataGridView2.EndEdit();
                bs.EndEdit();
                if (dtextra.Rows.Count > 0)
                {
                    for (int i = dtextra.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow fila = dtextra.Rows[i];
                        bool vacia = fila.ItemArray.All(c => c == null || string.IsNullOrWhiteSpace(c.ToString()));
                        if (vacia) dtextra.Rows.RemoveAt(i);
                    }

                    int contador = 0;
                    foreach (DataRow x in dtextra.Rows)
                    {
                        string mes, dias, monto;
                        mes = x["Mes"].ToString();
                        dias = x["Dia"].ToString();
                        monto = x["Monto"].ToString();

                        if (string.IsNullOrEmpty(mes) || string.IsNullOrEmpty(dias) || string.IsNullOrEmpty(monto))
                        {
                            dtextra.Rows.RemoveAt(contador);
                            contador++;
                        }
                    }

                    if (contador > 0)
                    {
                        error(dataGridView2, "Llene todos los campos");
                    }

                    dv = dtextra.DefaultView;
                    dv.Sort = "Mes DESC";
                    CuotasExtras = dv.ToTable();

                }
            }

            string[] linea = new string[6];
            if (!string.IsNullOrEmpty(txtCuota.Text))
            {
                double InteresMes = 0;
                double Saldo = 0;
                double CapitalVigente =0;
                double CuotaFinal=0;
                TiempoDeVida = 0;
                for (int i = 0; i < TPlazo; i++)
                {
                    if(i == 0)
                    {
                        linea[0] = (i + 1).ToString();
                        linea[1] = TCuota.ToString();
                        InteresMes = CalcularInteres(TMonto, TTasa);
                        linea[2] = InteresMes.ToString("N2");
                        linea[3] = TSeguro.ToString("N2");
                        Saldo = Math.Round(TCuota - TSeguro - InteresMes,2);
                        linea[4] = Saldo.ToString("N2");
                        CapitalVigente = Math.Round(TMonto - Saldo,2);
                        linea[5] = CapitalVigente.ToString("N2");
                        dt.Rows.Add(linea);
                    }
                    else if (i == TPlazo - 1)
                    {
                        linea[0] = (i + 1).ToString();
                        linea[1] = TCuota.ToString("N2");
                        InteresMes = CalcularInteres(CapitalVigente, TTasa);
                        linea[2] = InteresMes.ToString("N2");
                        linea[3] = TSeguro.ToString("N2");
                        Saldo = Math.Round(TCuota - TSeguro - InteresMes, 2);
                        linea[4] = Saldo.ToString("N2");
                        CapitalVigente = Math.Round(CapitalVigente - Saldo, 2);
                        if(CapitalVigente > 0)
                        {
                            InteresMes = (InteresMes - CapitalVigente);
                            Saldo = Math.Round(TCuota - TSeguro - InteresMes, 2);
                            CapitalVigente = 0.0;
                            linea[2] = InteresMes.ToString("N2");
                            linea[4] = Saldo.ToString("N2");
                            linea[5] = CapitalVigente.ToString("N2");
                        }

                        dt.Rows.Add(linea);
                    }
                    else
                    {

                        if (TCuota > CapitalVigente + TSeguro + CalcularInteres(CapitalVigente, TTasa))
                        {
                            linea[0] = (i + 1).ToString();
                            CuotaFinal = CapitalVigente + TSeguro + CalcularInteres(CapitalVigente, TTasa);
                            linea[1] = CuotaFinal.ToString();
                            InteresMes = CalcularInteres(CapitalVigente, TTasa);
                            linea[2] = InteresMes.ToString("N2");
                            linea[3] = TSeguro.ToString("N2");
                            Saldo = Math.Round(CuotaFinal - TSeguro - InteresMes, 2);
                            linea[4] = Saldo.ToString("N2");
                            CapitalVigente = Math.Round(CapitalVigente - Saldo, 2);
                            linea[5] = CapitalVigente.ToString("N2");
                            dt.Rows.Add(linea);
                            i = TPlazo;
                        }
                        else
                        {
                            linea[0] = (i + 1).ToString();
                            linea[1] = TCuota.ToString("N2");
                            InteresMes = CalcularInteres(CapitalVigente, TTasa);
                            linea[2] = InteresMes.ToString("N2");
                            linea[3] = TSeguro.ToString("N2");
                            Saldo = Math.Round(TCuota - TSeguro - InteresMes, 2);
                            linea[4] = Saldo.ToString("N2");
                            CapitalVigente = Math.Round(CapitalVigente - Saldo, 2);
                            linea[5] = CapitalVigente.ToString("N2");

                            dt.Rows.Add(linea);
                        }
                        
                    }

                    if (CuotasExtras.Rows.Count > 0)
                    {
                        foreach (DataRow z in CuotasExtras.Rows)
                        {
                            if (i+1 == Convert.ToUInt32(z["Mes"].ToString()))
                            {

                                linea[0] = string.Concat((i+1).ToString(), ".", z["Dia"].ToString());
                                linea[1] = Convert.ToDouble(z["Monto"].ToString()).ToString();
                                InteresMes = CalcularInteresPorDias(CapitalVigente, TTasa, Convert.ToInt32(z["Dia"].ToString()));
                                linea[2] = InteresMes.ToString("N2");
                                linea[3] = (0.0).ToString();
                                Saldo = Math.Round(Convert.ToDouble(z["Monto"].ToString()) - 0.0 - InteresMes, 2);
                                linea[4] = Saldo.ToString("N2");
                                CapitalVigente = Math.Round(CapitalVigente - Saldo, 2);
                                linea[5] = CapitalVigente.ToString("N2");

                                if(CapitalVigente < 0)
                                {
                                    i = TPlazo;
                                }

                                dt.Rows.Add(linea);
                            }
                        }
                    }

                    TiempoDeVida++;
                }
                dataGridView1.DataSource = dt;   
                InteresAcumulado = Convert.ToDouble(dt.Compute("SUM(Interes)", ""));
                SeguroAcomulado = Convert.ToDouble(dt.Compute("SUM(Seguro)", ""));

                txtInteresPagado.Text = string.Concat("$",InteresAcumulado.ToString());
                OtrosPagado.Text = string.Concat("$", SeguroAcomulado.ToString());
                txtTiempo.Text = TiempoDeVida.ToString();

                dataGridView1.Columns["Cuota"].DefaultCellStyle.Format = "C2";
                dataGridView1.Columns["Interes"].DefaultCellStyle.Format = "C2";
                dataGridView1.Columns["Seguro"].DefaultCellStyle.Format = "C2";
                dataGridView1.Columns["Saldo"].DefaultCellStyle.Format = "C2";
                dataGridView1.Columns["Capital"].DefaultCellStyle.Format = "C2";

            }
            else
            {
                error(txtCuota, "Debe que tener antes una cuota para calcular la tabla");
            }
        }


    }
}
