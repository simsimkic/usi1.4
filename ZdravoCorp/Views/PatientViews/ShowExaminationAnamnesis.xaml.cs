using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZdravoCorp.Models.Entities.Patients;

namespace ZdravoCorp.Views.PatientViews
{
    public partial class ShowExaminationAnamnesis : Window
    {
        public Anamnesis SelectedAnamnesis { get; set; }
        public ShowExaminationAnamnesis(Anamnesis anamnesis)
        {
            InitializeComponent();
            SelectedAnamnesis = anamnesis;
            TextBoxIdAnamnesis.Text = anamnesis.Id.ToString();
            TextBoxIdExamination.Text = anamnesis.ExaminationId.ToString();
            TextBoxDisease.Text = anamnesis.Disease;
            TextBoxSymptoms.Text = anamnesis.Symptoms;
            TextBoxAllergens.Text = anamnesis.Allergens;
        }
    }
}
