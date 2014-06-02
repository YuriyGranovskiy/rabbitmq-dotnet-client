// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (C) 2007-2014 GoPivotal, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License.
//
//  The Original Code is RabbitMQ.
//
//  The Initial Developer of the Original Code is GoPivotal, Inc.
//  Copyright (c) 2007-2014 GoPivotal, Inc.  All rights reserved.
//---------------------------------------------------------------------------

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Threading;

using RabbitMQ.Client.Exceptions;

namespace RabbitMQ.Client.Unit {
    [TestFixture]
    public class TestQueueDeclare : IntegrationFixture {

        [Test]
        public void TestConcurrentQueueDeclare()
        {
            string q = GenerateQueueName();
            Random rnd = new Random();

            List<Thread> ts = new List<Thread>();
            System.NotSupportedException nse = null;
            for(int i = 0; i < 256; i++)
            {
                Thread t = new Thread(() =>
                        {
                            try
                            {
                                // sleep for a random amount of time to increase the chances
                                // of thread interleaving. MK.
                                Thread.Sleep(rnd.Next(5, 500));
                                Model.QueueDeclare(q, false, false, false, null);
                            } catch (System.NotSupportedException e)
                            {
                                nse = e;
                            }
                        });
                ts.Add(t);
                t.Start();
            }

            foreach (Thread t in ts)
            {
                t.Join();
            }

            Assert.IsNotNull(nse);
            Model.QueueDelete(q);
        }

		[Test]
		public void TestQueueDeclareNowait()
		{
			string q = GenerateQueueName ();
			Model.QueueDeclareNowait(q, false, true, false, null);
			Model.QueueDeclarePassive(q);
		}
    }
}
