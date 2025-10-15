using System;
using System.Collections.Generic;
using System.Linq;

using IFM.PolicyLoader.QuickQuote;

using NSubstitute;

using QuickQuote.CommonObjects;

using Xunit;

namespace IFM.PolicyLoader.Tests
{
    public class UnderlyingPolicyPostOpHandlerProviderTests
    {
        List<IPostOperationHandler> _handlers;

        public UnderlyingPolicyPostOpHandlerProviderTests()
        {
            //setup default handlers collection

        }

        [Fact]
        public void CanProvideSuppliedHandlerIfCriteriaMatch()
        {
            //arrange
            var qqObj1 = new QuickQuoteObject() { 
                LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal
            };
            var handler = Substitute.For<IPostOperationHandler>();
            handler.CanHandle(Arg.Any<QuickQuoteObject>()).Returns<bool>((caller) => caller.Arg<QuickQuoteObject>().LobType == QuickQuoteObject.QuickQuoteLobType.AutoPersonal);
            IPostOperationHandlerProvider provider = new UnderlyingPolicyPostOpHandlerProvider(handler);
            //act
            var retval = provider.CanProvideFor(qqObj1);
            
            //assert
            Assert.True(retval);
            Assert.True(handler.Received().CanHandle(Arg.Any<QuickQuoteObject>()));
        }
    }
}
