using FluentValidation;
using NAME.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.API.Validation
{
    internal class SendManifestDTOValidator : AbstractValidator<SendManifestDTO>
    {
        public SendManifestDTOValidator()
        {
            this.RuleFor(x => x.Manifest).NotEmpty();
        }
    }
}
