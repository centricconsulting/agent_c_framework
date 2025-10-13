using IFM.VR.Common.Underwriting;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using QuickQuote.CommonObjects;

using System;
using System.Linq;

using static QuickQuote.CommonMethods.QuickQuoteHelperClass;
using static QuickQuote.CommonObjects.QuickQuoteObject;

namespace VrTests.VR_Common.UWQuestions
{
    [TestClass]
    public class UnderwritingQuestionsService_Tests //need to create a base class for the common items
    {
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private UnderwritingQuestionsService setupService()
        {
            return new UnderwritingQuestionsService(new MemoryCache(new MemoryCacheOptions()), new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory), new QuickQuote.CommonMethods.QuickQuoteHelperClass());
        }

        private QuickQuoteObject setupQuickQuoteObject_withEmptyMultiStateQuote(QuickQuoteState governingState, QuickQuoteLobType lobType)
        {
            var retval = new QuickQuoteObject();

            retval.MultiStateQuotes = new System.Collections.Generic.List<QuickQuoteObject>();
            retval.MultiStateQuotes.Add(new QuickQuoteObject());

            var msQuote = retval.MultiStateQuotes[0];

            retval.QuickQuoteState= governingState;
            msQuote.QuickQuoteState = governingState;

            retval.LobType = lobType;
            msQuote.LobType = lobType;

            return retval;
        }

        [TestMethod()]
        public void GetQuestions_ThrowsExceptionWhenLobTypeIsNotSetInRequest()
        {
            //arrange
            IUnderwritingQuestionsService svc = setupService();
            UnderwritingQuestionRequest request = new UnderwritingQuestionRequest();

            //act & assert
            Assert.ThrowsException<ArgumentException>(() => svc.GetQuestions(request));
        }


        [TestMethod()]
        public void GetQuestions_ThrowsExceptionWhenRequestIsNull()
        {
            //arrange
            IUnderwritingQuestionsService svc = setupService();
            UnderwritingQuestionRequest request = null;

            //act & assert
            Assert.ThrowsException<ArgumentNullException>(() => svc.GetQuestions(request));
        }

        [TestMethod]
        public void RetrievesQuestions_AutoPersonal_Ohio_GoverningStateOnly ()
        {
            //arrange
            IUnderwritingQuestionsService svc = setupService();
            UnderwritingQuestionRequest request = new UnderwritingQuestionRequest()
            {
                LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal,
                GoverningState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio,
                TypeFilter = UnderwritingQuestionRequest.QuestionTypeFilter.GoverningStateOnly
            };
            //act
            var result = svc.GetQuestions(request);

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void RetrievesQuestions_AutoPersonal_Ohio_ExcludingUnmapped()
        {
            //arrange
            IUnderwritingQuestionsService svc = setupService();
            UnderwritingQuestionRequest request = new UnderwritingQuestionRequest()
            {
                LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal,
                GoverningState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio,
                TypeFilter = UnderwritingQuestionRequest.QuestionTypeFilter.GoverningStateOnly | UnderwritingQuestionRequest.QuestionTypeFilter.ExcludeUnmapped
            };
            //act
            var result = svc.GetQuestions(request);

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.All(q => q.IsUnmapped == false));
        }

        [TestMethod]
        public void RetrievesKillQuestions_AutoPersonal_Ohio()
        {
            //arrange
            IUnderwritingQuestionsService svc = setupService();
            UnderwritingQuestionRequest request = new UnderwritingQuestionRequest()
            {
                LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal,
                GoverningState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio,
                TypeFilter = UnderwritingQuestionRequest.QuestionTypeFilter.KillOnly
            };
            //act
            var result = svc.GetQuestions(request);

            //assert
            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.All(q=>q.IsTrueKillQuestion));
        }

        [TestMethod]
        public void SavesAnswersToQuote_AutoPersonal_Ohio()
        {
            //arrange
            IUnderwritingQuestionsService svc = setupService();
            UnderwritingQuestionRequest request = new UnderwritingQuestionRequest()
            {
                LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal,
                GoverningState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio,
                TypeFilter = UnderwritingQuestionRequest.QuestionTypeFilter.GoverningStateOnly | UnderwritingQuestionRequest.QuestionTypeFilter.ExcludeUnmapped
            };
            var quote = setupQuickQuoteObject_withEmptyMultiStateQuote(QuickQuoteState.Ohio, QuickQuoteLobType.AutoPersonal);
            
           
            var questions = svc.GetQuestions(request).ToList();
            

            foreach(var q in questions)
            {
                if (random.Next()%2==0) //if number is even
                {
                    q.QuestionAnswerYes = true;
                    q.DetailTextOnQuestionYes = RandomString(50);
                }
                else
                {
                    q.QuestionAnswerNo = true;
                }
            }

            var savRequest = new UnderwritingSaveRequest
            {
                GoverningState = QuickQuoteState.Ohio,
                LobType = QuickQuoteLobType.AutoPersonal,
                Answers = questions,
                Quote = quote
            };
            //act
            var result = svc.SaveAnswers(savRequest);
            //assert
            Assert.IsTrue(result);
            //Assert.IsTrue(questions.)
        }
    }
}
