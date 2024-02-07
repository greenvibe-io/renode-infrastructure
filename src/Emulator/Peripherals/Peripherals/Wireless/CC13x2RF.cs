//
// Copyright (c) 2010-2023 Antmicro
// Copyright (c) 2011-2015 Realtime Embedded
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//
using System.Linq;
using Antmicro.Renode.Peripherals.Bus;
using Antmicro.Renode.Logging;
using System;
using System.Collections.Generic;
using Antmicro.Renode.Core;
using Antmicro.Renode.Utilities;
using Antmicro.Migrant;
using Antmicro.Renode.Peripherals.Wireless.IEEE802_15_4;
using Antmicro.Renode.Core.Structure.Registers;

// CC13x2RF.cs(21,81): error CS0535: 'CC13x2RF' does not implement interface member 'IRadio.ReceiveFrame(byte[])'
// CC13x2RF.cs(21,81): error CS0535: 'CC13x2RF' does not implement interface member 'IRadio.Channel'
// CC13x2RF.cs(21,81): error CS0535: 'CC13x2RF' does not implement interface member 'IRadio.FrameSent'

namespace Antmicro.Renode.Peripherals.Wireless
{
    public class CC13x2RF : IDoubleWordPeripheral, IBytePeripheral, IKnownSize, IRadio
    {
        public CC13x2RF()
        {
            IRQ = new GPIO();
            irqHandler = new InterruptHandler<InterruptRegister, InterruptSource>(IRQ);
            irqHandler.RegisterInterrupt(InterruptRegister.IrqFlag, InterruptSource.Poked, 0);

            var pokeme = new DoubleWordRegister(this, 0x0);
            var addresses = new Dictionary<long, DoubleWordRegister>
            {
                { (uint)Register.Pokeme, pokeme },
            };

            registers = new DoubleWordRegisterCollection(this, addresses);

            Reset();
        }

        public uint ReadDoubleWord(long offset)
        {
            uint result = 0u;
            result = registers.Read(offset);

            return result;
        }

        public void WriteDoubleWord(long offset, uint value)
        {
            registers.Write(offset, value);
            irqHandler.RequestInterrupt(InterruptSource.Poked);

            return;
        }

        public void ReceiveFrame(byte[] bytes)
        {
        }


        //used by uDMA
        public byte ReadByte(long offset)
        {
            return 0;
        }

        //used by uDMA
        public void WriteByte(long offset, byte value)
        {
        }

        public void Reset()
        {
            registers.Reset();
        }


        public long Size { get { return 0x4000; } }
        public int Channel { get; set; }
        public event Action<IRadio, byte[]> FrameSent;
        private readonly InterruptHandler<InterruptRegister, InterruptSource> irqHandler;
        private readonly DoubleWordRegisterCollection registers;
        // should be private set, but easier to debug by making it public
        public GPIO IRQ { get; set; }
        
        private enum Register
        {
            Pokeme = 0x0,
        }

        private enum InterruptSource
        {
            Poked = 0x0,
        }

        private enum InterruptRegister
        {
            IrqFlag,
        }
    }
}

