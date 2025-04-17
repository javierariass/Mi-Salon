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
            DateTime fechaInicio = DateTime.Parse(dateTimePicker1.Text);
            DateTime fechaFin = DateTime.Parse(dateTimePicker2.Text);

            // Obtener los clientes
            var clientes = Functions.ObtenerClientes(appDataPath, fechaInicio, fechaFin);
            var rebooking = Functions.ObtenerRebooking(appDataPath, fechaInicio, fechaFin);

            InformeReservas informe = new InformeReservas(clientes, rebooking, fechaInicio.ToString("yyyy-MM-dd"), fechaFin.ToString("yyyy-MM-dd"));

            // Generar el PDF
            GenerarPDF(informe);
        }

        //Menu de reporte de clientes nuevos
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

                        Paragraph newClient = new Paragraph("Listado de nuevos clientes: ");
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
                        doc.Add(new Paragraph(" "));
                        doc.Add(new Paragraph(" "));
                        Paragraph newRebooking = new Paragraph("Cantidad de Clientes y Porciento Rebooking: ");
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

    public InformeReservas(List<(string Nombre, string Peluquero, string Fecha)> clientes,
                           List<(string Peluquero, string TotalReservas, string PorcentajeRebooking)> rebooking,
                           string fechaInicio, string fechaFinal)
    {
        Clientes = clientes;
        Rebooking = rebooking;
        FechaInicio = fechaInicio;
        FechaFinal = fechaFinal;
    }
}
