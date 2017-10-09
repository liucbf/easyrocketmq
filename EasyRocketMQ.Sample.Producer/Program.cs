﻿using EasyRocketMQ.Producers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EasyRocketMQ.Sample.Producer
{
    internal class Program
    {
        private static readonly string AccessKeyId = "LTAIspikO0gRSqC5";

        private static readonly string AccessKeySecret = "Y3l9MVqQsdM2OnOTs7v5dalcPNZXGJ";

        /// <summary>
        /// 每线程发送消息数量
        /// </summary>
        private static readonly int MessageCountPerThread = 10;

        private static readonly string ProducerId = "PID_testpay_producer1";

        /// <summary>
        /// 线程总数
        /// </summary>
        private static readonly int ProducerThreadCount = 20;

        private static readonly string Topic = "testpay";

        /// <summary>
        /// 需要静态对象
        /// </summary>
        private static OrderProducerClient producerClient = new OrderProducerClient(AccessKeyId, AccessKeySecret, ProducerId);

        private static void Main(string[] args)
        {
            producerClient.Start();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var taskList = new List<Task>();
            for (int threadIndex = 1; threadIndex <= ProducerThreadCount; threadIndex++)
            {
                // 生产消费
                var task = Task.Factory.StartNew(() => {
                    for (int messageIndex = 1; messageIndex <= MessageCountPerThread; messageIndex++)
                    {
                        string content = "线程ID=" + Thread.CurrentThread.ManagedThreadId + ", 我要测试rocketmq message";
                        producerClient.SendMessage(Topic, content, "MQ");

                        Console.WriteLine(content);
                    }
                }, TaskCreationOptions.LongRunning);

                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());
            stopWatch.Stop();

            // 一定要关闭，不然会有内存泄漏
            producerClient.Shutdown();

            Console.WriteLine("发送消息：{0}条， 使用时间{1}毫秒", MessageCountPerThread * ProducerThreadCount, stopWatch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}