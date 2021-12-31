using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seq2SeqSharp;
using Seq2SeqSharp.Tools;
using Seq2SeqSharp.Corpus;
using Seq2SeqSharp._SentencePiece;
using Seq2SeqSharp.Applications;

namespace Seq2SeqWebApps
{
    public static class Seq2SeqInstance
    {
        static private Seq2Seq m_seq2seq;
        static private SentencePiece m_srcSpm;
        static private SentencePiece m_tgtSpm;
        static private Seq2SeqOptions opts;

        static public void Initialization(string modelFilePath, int maxTestSrcSentLength, int maxTestTgtSentLength, string deviceIds, SentencePiece srcSpm, SentencePiece tgtSpm,
            Seq2SeqSharp.Utils.DecodingStrategyEnums decodingStrategyEnum, float topPSampling, float distancePenalty, float repeatPenalty)
        {
            opts = new Seq2SeqOptions();
            opts.ModelFilePath = modelFilePath;
            opts.MaxTestSrcSentLength = maxTestSrcSentLength;
            opts.MaxTestTgtSentLength = maxTestTgtSentLength;
            opts.ProcessorType = ProcessorTypeEnums.CPU;
            opts.DeviceIds = deviceIds;
            opts.DecodingStrategy = decodingStrategyEnum;
            opts.DecodingDistancePenalty = distancePenalty;
            opts.DecodingRepeatPenalty = repeatPenalty;
            opts.DecodingTopPValue = topPSampling;

            m_srcSpm = srcSpm;
            m_tgtSpm = tgtSpm;

            m_seq2seq = new Seq2Seq(opts);
        }

        static public string Call(string input, int tokenNumToGenerate, bool random, float distancePenalty, float repeatPenalty)
        {
            input = m_srcSpm.Encode(input);
            List<string> tokens = input.Split(' ').ToList();
            tokenNumToGenerate += tokens.Count;

            List<List<String>> batchTokens = new List<List<string>>();
            batchTokens.Add(tokens);

            List<List<List<string>>> groupBatchTokens = new List<List<List<string>>>();
            groupBatchTokens.Add(batchTokens);



            List<string> tokens2 = input.Split(' ').ToList();

            List<List<String>> batchTokens2 = new List<List<string>>();
            batchTokens2.Add(tokens2);

            List<List<List<string>>> groupBatchTokens2 = new List<List<List<string>>>();
            groupBatchTokens2.Add(batchTokens2);


            DecodingOptions decodingOptions = opts.CreateDecodingOptions();
            decodingOptions.MaxTgtSentLength = tokenNumToGenerate;
            decodingOptions.TopPValue = random ? 0.9f : 0.0f;
            decodingOptions.DistancePenalty = distancePenalty;
            decodingOptions.RepeatPenalty = repeatPenalty;

            var nrs = m_seq2seq.Test<Seq2SeqCorpusBatch>(groupBatchTokens, groupBatchTokens2, decodingOptions);
            string rst = String.Join(" ", nrs[0].Output[0][0].ToArray(), 1, nrs[0].Output[0][0].Count - 2);
            rst = m_tgtSpm.Decode(rst);

            return rst;
        }
    }
}
