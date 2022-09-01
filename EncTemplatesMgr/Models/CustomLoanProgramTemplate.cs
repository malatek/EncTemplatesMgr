using EllieMae.EMLite.DataEngine;
using EllieMae.Encompass.BusinessObjects.Loans.Templates;
using EncTemplatesMgr.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncTemplatesMgr.Models
{
    public class CustomLoanProgramTemplate : Interfaces.ICustomTemplate
    {
        /// <summary>
        /// Template Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Template Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// File path where template would reside in Encompass.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Path to the associated Closing Cost Template.
        /// </summary>
        public string ClosingCostPath { get; set; }

        /// <summary>
        /// Ignore field access rules.
        /// </summary>
        public bool IgnoreBusinessRules { get; set; }

        /// <summary>
        /// Linked Plan Code.
        /// </summary>
        public string PlanCode { get; set; }

        /// <summary>
        /// Linked Plan Code description.
        /// </summary>
        public string PlanDescription { get; set; }

        /// <summary>
        /// Linked Plan Code plan ID.
        /// </summary>
        public string PlanID { get; set; }

        /// <summary>
        /// Linked Plan Code Investor.
        /// </summary>
        public string PlanInvestor { get; set; }

        private Dictionary<string, string> _fieldIDsAndValues;

        /// <summary>
        /// Field IDs and their associated values.
        /// </summary>
        public Dictionary<string, string> FieldIDsAndValues
        {
            get
            {
                if (this._fieldIDsAndValues == null)
                {
                    this._fieldIDsAndValues = new Dictionary<string, string>();
                }
                return this._fieldIDsAndValues;
            }

            set { this._fieldIDsAndValues = value; }
        }

        /// <summary>
        /// Cast LoanProgram template to CustomLoanProgramTemplate.
        /// </summary>
        /// <param name="loanProgram"></param>
        public static explicit operator CustomLoanProgramTemplate(EllieMae.EMLite.DataEngine.LoanProgram loanProgram)
        {
            var customLoanProgramTemplate = new CustomLoanProgramTemplate()
            {
                Name = loanProgram.TemplateName,
                Description = loanProgram.Description,
                ClosingCostPath = loanProgram.ClosingCostPath,
                PlanCode = loanProgram.PlanCode,
                PlanDescription = loanProgram.PlanDescription,
                PlanID = loanProgram.PlanID,
                PlanInvestor = loanProgram.PlanInvestor
            };

            var fieldsAndValues = new Dictionary<string, string>();
            foreach (var field in loanProgram.GetAssignedFieldIDs())
            {
                var fieldValue = loanProgram.GetField(field);
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    fieldsAndValues.Add(field, fieldValue);
                }
            }

            customLoanProgramTemplate.FieldIDsAndValues = fieldsAndValues;

            return customLoanProgramTemplate;
        }

        /// <summary>
        /// Convert CustomLoanProgramTemplate to LoanProgram template.
        /// </summary>
        /// <returns></returns>
        public EllieMae.EMLite.DataEngine.LoanProgram ToLoanProgram()
        {
            var loanProgram = new EllieMae.EMLite.DataEngine.LoanProgram()
            {
                TemplateName = this.Name,
                Description = this.Description,
                ClosingCostPath = this.ClosingCostPath,
                PlanCode = this.PlanCode,
                PlanDescription = this.PlanDescription,
                PlanID = this.PlanID,
                PlanInvestor = this.PlanInvestor
            };

            foreach (var kvp in this.FieldIDsAndValues)
            {
                try
                {
                    loanProgram.SetField(kvp.Key, kvp.Value);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error("TemplateImport", ex);
                }
            }

            return loanProgram;
        }
    }
}
