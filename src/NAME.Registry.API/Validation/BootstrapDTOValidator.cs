using FluentValidation;
using NAME.Core.DTOs;
using NAME.Core;

namespace NAME.Registry.API.Validation
{
    /// <summary>
    /// Validator for the <see cref="BootstrapDTO"/>.
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{T}" />
    public class BootstrapDTOValidator : AbstractValidator<BootstrapDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapDTOValidator"/> class.
        /// </summary>
        public BootstrapDTOValidator()
        {
            this.RuleFor(x => x.SupportedProtocols).NotNull();
            this.RuleFor(x => x.Hostname).NotEmpty();
            this.RuleFor(x => x.NAMEEndpoint).NotEmpty();
            this.RuleFor(x => x.NAMEPort).Must(p => p == null || p > 0u);
            this.RuleFor(x => x.AppVersion).Must(v => DependencyVersion.TryParse(v, out var version)).WithMessage("Version is not valid.");
            this.RuleFor(x => x.NAMEVersion).Must(v => DependencyVersion.TryParse(v, out var version)).WithMessage("Version is not valid.");
            this.RuleForEach(x => x.SupportedProtocols).GreaterThan(0u);
        }
    }
}
