using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace LokerMVC.Attribute;

public class DateRangeValidatorAttribute(string propertyName) : ValidationAttribute, IClientModelValidator
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return null;

        var tglAkhir = Convert.ToDateTime(value);
        var otherProperty = validationContext.ObjectType.GetProperty(propertyName);

        if (otherProperty == null)
            return null;

        var tglAwal = Convert.ToDateTime(otherProperty.GetValue(validationContext.ObjectInstance));

        if (tglAkhir > tglAwal)
        {
            return new ValidationResult(ErrorMessage, new[] { propertyName, validationContext.MemberName });
        }

        return ValidationResult.Success;

    }


    public void AddValidation(ClientModelValidationContext context)
    {
        var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());

        // Menambahkan pesan kesalahan validasi
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-daterange", errorMessage);

        // Menambahkan properti nama untuk validasi
        context.Attributes.Add("data-val-daterange-property", JsonConvert.ToString(propertyName));

        // Menambahkan properti tanggal awal untuk validasi
        context.Attributes.Add("data-val-daterange-startdate", JsonConvert.ToString(propertyName));

        // Menambahkan properti tanggal akhir untuk validasi
        context.Attributes.Add("data-val-daterange-enddate", JsonConvert.ToString(context.ModelMetadata.PropertyName));

        // Menambahkan skrip JavaScript untuk validasi klien
        var script = @"
        <script>
        $.validator.addMethod('daterange', function(value, element, params) {
            var startDate = $(params).val();
            var endDate = value;

            if (!startDate || !endDate) {
                return true;
            }

            startDate = new Date(startDate);
            endDate = new Date(endDate);

            return startDate < endDate;
        }, 'Tanggal awal harus lebih kecil dari tanggal akhir');

        $.validator.unobtrusive.adapters.add('daterange', ['property', 'startdate'], function(options) {
            options.rules['daterange'] = '#' + options.params.startdate;
            options.messages['daterange'] = options.message;
        });
        </script>";

        // Menambahkan skrip JavaScript ke atribut untuk dijalankan di sisi klien
        context.Attributes.Add("script", script);
    }
}