﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seq2SeqSharp;
using Seq2SeqSharp.Tools;
using Seq2SeqSharp.Corpus;
using Seq2SeqSharp._SentencePiece;

namespace Seq2SeqWebApps
{
    public static class Seq2SeqInstance
    {
        static private object locker = new object();
        static private Seq2Seq m_seq2seq;
        static private SentencePiece m_srcSpm;
        static private SentencePiece m_tgtSpm;

        static public void Initialization(string modelFilePath, int maxTestSrcSentLength, int maxTestTgtSentLength, string processorType, string deviceIds, SentencePiece srcSpm, SentencePiece tgtSpm, string decodingStrategy = "Sampling")
        {
            Seq2SeqOptions opts = new Seq2SeqOptions();
            opts.ModelFilePath = modelFilePath;
            opts.MaxTestSrcSentLength = maxTestSrcSentLength;
            opts.MaxTestTgtSentLength = maxTestTgtSentLength;
            opts.ProcessorType = processorType;
            opts.DeviceIds = deviceIds;
            opts.DecodingStrategy = decodingStrategy;

            m_srcSpm = srcSpm;
            m_tgtSpm = tgtSpm;

            m_seq2seq = new Seq2Seq(opts);
        }

        static public string Call(string input, int tokenNumToGenerate)
        {
            lock (locker)
            {

                input = m_srcSpm.Encode(input);
                List<string> tokens = input.Split(' ').ToList();
                tokenNumToGenerate += tokens.Count;

                List<List<String>> batchTokens = new List<List<string>>();
                batchTokens.Add(tokens);

                List<List<List<string>>> groupBatchTokens = new List<List<List<string>>>();
                groupBatchTokens.Add(batchTokens);

                m_seq2seq.SetMaxOutputTokenNum(tokenNumToGenerate);
                var nrs = m_seq2seq.Test<Seq2SeqCorpusBatch>(groupBatchTokens);
                string rst = String.Join(" ", nrs[0].Output[0][0].ToArray(), 1, nrs[0].Output[0][0].Count - 2);
                rst = m_tgtSpm.Decode(rst);

                return rst;
            }
        }
    }
}
