using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Shipping.CentralTransport.Services;

namespace Nop.Plugin.Shipping.CentralTransport.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => int.MaxValue;

        public void Register(
            ContainerBuilder builder,
            ITypeFinder typeFinder,
            NopConfig config)
        {
            builder.RegisterType<RateQuoteService>()
                        .As<IRateQuoteService>()
                        .InstancePerLifetimeScope();
        }
    }
}
