using Mi_Salon.Modulos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Runtime.Remoting;

namespace Mi_Salon
{
    public partial class Reporte : Form
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MiSalon.db";

        public Reporte()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime fechaInicio = DateTime.Parse(dateTimePicker1.Text);
                DateTime fechaFin = DateTime.Parse(dateTimePicker2.Text);

                // Obtener los clientes
                var clientes = Functions.ObtenerClientes(appDataPath, fechaInicio, fechaFin);

                var rebooking = Functions.ObtenerRebooking(appDataPath, fechaInicio, fechaFin);

                var ventas = Functions.ObtenerVentasPorPeluquero(appDataPath, fechaInicio, fechaFin);

                InformeReservas informe = new InformeReservas(clientes, rebooking, ventas, fechaInicio.ToString("yyyy-MM-dd"), fechaFin.ToString("yyyy-MM-dd"));

                // Generar el PDF
                GenerarPDF(informe);
            }
            catch
            {
                MessageBox.Show("Sistema ocupado.Cierre el documento del reporte y vuelva a intentar","Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Menu de reporte 
        static void GenerarPDF(InformeReservas informe)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveFileDialog.Title = "Guardar Informe como PDF";
                saveFileDialog.DefaultExt = "pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string archivoPDF = saveFileDialog.FileName;

                    using (FileStream fs = new FileStream(archivoPDF, FileMode.Create))
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, fs);

                        doc.Open();

                        // Agregar el título para nuevos clientes
                        Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA, 18); // Establecer tamaño 18
                        Paragraph titulo = new Paragraph(informe.FechaInicio + " hasta " + informe.FechaFinal, tituloFont);
                        titulo.Alignment = Element.ALIGN_CENTER; // Centrar el texto
                        doc.Add(titulo);
                        doc.Add(new Paragraph(" "));
                        doc.Add(new Paragraph(" "));


                        Font Font = FontFactory.GetFont(FontFactory.HELVETICA, 15); // Establecer tamaño 15
                        Paragraph newClient = new Paragraph("Listado de nuevos clientes: ",Font);
                        newClient.Alignment = Element.ALIGN_CENTER;
                        doc.Add(newClient);
                        doc.Add(new Paragraph(" "));

                        string peluqueroActual = null;

                        // Nuevos Clientes
                        foreach (var cliente in informe.Clientes)
                        {
                            if (cliente.Peluquero != peluqueroActual)
                            {
                                // Cambió el peluquero, agregar nuevo título
                                peluqueroActual = cliente.Peluquero;
                                doc.Add(new Paragraph(" "));
                                doc.Add(new Paragraph($"Peluquero: {peluqueroActual}"));
                            }

                            // Agregar al cliente debajo del peluquero correspondiente
                            doc.Add(new Paragraph($"- {cliente.Nombre} {cliente.Fecha}"));
                        }

                        peluqueroActual = null;
                        doc.NewPage();
                        Paragraph newRebooking = new Paragraph("Cantidad de Clientes y Porciento Rebooking: ",Font);
                        newRebooking.Alignment = Element.ALIGN_CENTER;
                        doc.Add(newRebooking);
                        doc.Add(new Paragraph(" "));

                        // % de rebooking
                        foreach (var rebooking in informe.Rebooking)
                        {
                            if (rebooking.Peluquero != peluqueroActual)
                            {
                                // Cambió el peluquero, agregar nuevo título
                                peluqueroActual = rebooking.Peluquero;
                                doc.Add(new Paragraph(" "));
                                doc.Add(new Paragraph($"Peluquero: {peluqueroActual}"));
                                doc.Add(new Paragraph($"-Cantidad de clientes: {rebooking.TotalReservas}"));
                                doc.Add(new Paragraph($"-Porciento de rebooking: {rebooking.PorcentajeRebooking}"));
                                doc.Add(new Paragraph(" "));
                            }
                        }

                        //Ventas totales por peluqueros
                        doc.NewPage();
                        Paragraph newVentas = new Paragraph("Ventas por peluqueros: ",Font);
                        newVentas.Alignment = Element.ALIGN_CENTER;
                        doc.Add(newVentas);
                        doc.Add(new Paragraph(" "));

                        // Ventas por peluquero

                        List<string> nombres = new List<string>();
                        foreach (var venta in informe.Ventas)
                        {
                            if (!nombres.Contains(venta.Peluquero))
                            {
                                nombres.Add(venta.Peluquero);
                                doc.Add(new Paragraph(" "));
                                doc.Add(new Paragraph($"Peluquero: {venta.Peluquero}"));
                                doc.Add(new Paragraph(" "));
                                for (int i = 0; i < informe.Ventas.Count;i++)
                                {
                                    if (informe.Ventas[i].Peluquero == venta.Peluquero)
                                    {
                                        doc.Add(new Paragraph($"- Cliente: {venta.Cliente}       Total de la venta: {venta.Total}         Fecha: {venta.Fecha}"));
                                    }
                                }                                                              
                            }                          
                        }


                        doc.Close();
                    }

                    MessageBox.Show("El PDF ha sido generado correctamente en: " + archivoPDF, "PDF Generado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}

public class InformeReservas
{
    public List<(string Nombre, string Peluquero, string Fecha)> Clientes { get; set; }
    public List<(string Peluquero, string TotalReservas, string PorcentajeRebooking)> Rebooking { get; set; }
    public string FechaInicio { get; set; }
    public string FechaFinal { get; set; }

    public List<(string Peluquero, string Cliente, string Total, string Fecha)> Ventas { get; set; }

    public InformeReservas(List<(string Nombre, string Peluquero, string Fecha)> clientes,
                           List<(string Peluquero, string TotalReservas, string PorcentajeRebooking)> rebooking, 
                           List<(string Peluquero, string Cliente, string Total, string Fecha)> ventas,
                           string fechaInicio, string fechaFinal)
    {
        Clientes = clientes;
        Rebooking = rebooking;
        FechaInicio = fechaInicio;
        FechaFinal = fechaFinal;
        Ventas = ventas;
    }
}
