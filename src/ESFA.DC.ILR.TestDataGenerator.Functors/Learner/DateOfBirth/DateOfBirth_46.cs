﻿using System;
using System.Collections.Generic;
using System.Linq;
using DCT.ILR.Model;

namespace DCT.TestDataGenerator.Functor
{
    public class DateOfBirth_46
        : ILearnerMultiMutator
    {
        private ILearnerCreatorDataCache _dataCache;
        private GenerationOptions _options;

        public FilePreparationDateRequired FilePreparationDate()
        {
            return FilePreparationDateRequired.None;
        }

        public IEnumerable<LearnerTypeMutator> LearnerMutators(ILearnerCreatorDataCache cache)
        {
            _dataCache = cache;
            return new List<LearnerTypeMutator>()
            {
                new LearnerTypeMutator() { LearnerType = LearnerTypeRequired.Apprenticeships, DoMutateLearner = Mutate19Standard, DoMutateOptions = MutateGenerationOptionsStandards, InvalidLines = 3 },
                new LearnerTypeMutator() { LearnerType = LearnerTypeRequired.Apprenticeships, DoMutateLearner = Mutate19StandardRestart, DoMutateOptions = MutateGenerationOptionsStandards, ExclusionRecord = true }
            };
        }

        public void MutateGenerationOptionsStandards(GenerationOptions options)
        {
            options.LD.OverrideLearnStartDate = DateTime.Parse("2016-SEP-30");
            options.CreateDestinationAndProgression = true;
            options.LD.IncludeHHS = true;
            _options = options;
        }

        public string RuleName()
        {
            return "DateOfBirth_46";
        }

        public string LearnerReferenceNumberStub()
        {
            return "DOB_46";
        }

        private void Mutate19Standard(MessageLearner learner, bool valid)
        {
            ApprenticeshipProgrammeTypeAim pta = _dataCache.ApprenticeshipAims(ProgType.ApprenticeshipStandard).First();
            learner.LearningDelivery[0].LearnStartDate = _options.LD.OverrideLearnStartDate.Value;
            MutateCommon(learner, valid);
            Helpers.SetApprenticeshipAims(learner, pta);
        }

        private void Mutate19StandardRestart(MessageLearner learner, bool valid)
        {
            Mutate19Standard(learner, valid);
            Helpers.AddLearningDeliveryRestartFAM(learner);
        }

        private void MutateCommon(MessageLearner learner, bool valid)
        {
            Helpers.MutateApprenticeshipToStandard(learner, FundModel.OtherAdult);
            Helpers.MutateDOB(learner, valid, Helpers.AgeRequired.Exact19, Helpers.BasedOn.LearnDelStart, Helpers.MakeOlderOrYoungerWhenInvalid.NoChange);
            Helpers.SetLearningDeliveryEndDates(learner.LearningDelivery[0], learner.LearningDelivery[0].LearnStartDate.AddDays(372), Helpers.SetAchDate.SetAchDate);
            Helpers.SetLearningDeliveryEndDates(learner.LearningDelivery[1], learner.LearningDelivery[0].LearnActEndDate, Helpers.SetAchDate.DoNotSetAchDate);

            if (!valid)
            {
                Helpers.SetLearningDeliveryEndDates(learner.LearningDelivery[0], learner.LearningDelivery[0].LearnStartDate.AddDays(364), Helpers.SetAchDate.SetAchDate);
            }
        }
    }
}
