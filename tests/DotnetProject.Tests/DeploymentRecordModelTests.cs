using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DotnetProject.Web.Models;
using Xunit;

namespace DotnetProject.Tests
{
    public class DeploymentRecordModelTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void ValidModel_PassesValidation()
        {
            // Arrange
            var record = new DeploymentRecord
            {
                AppName = "DotnetProject.Web",
                Version = "v1.0.0",
                Environment = "Production",
                Status = "Successful",
                DeployedBy = "GitHub Actions",
                CommitHash = "c1a2b3d"
            };

            // Act
            var results = ValidateModel(record);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void MissingAppName_FailsValidation()
        {
            // Arrange
            var record = new DeploymentRecord
            {
                AppName = "", // Required
                Version = "v1.0.0",
                Environment = "Production",
                Status = "Successful"
            };

            // Act
            var results = ValidateModel(record);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(DeploymentRecord.AppName)));
        }

        [Fact]
        public void MissingVersion_FailsValidation()
        {
            // Arrange
            var record = new DeploymentRecord
            {
                AppName = "DotnetProject.Web",
                Version = "", // Required
                Environment = "Production",
                Status = "Successful"
            };

            // Act
            var results = ValidateModel(record);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(DeploymentRecord.Version)));
        }
    }
}
