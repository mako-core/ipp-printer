// -----------------------------------------------------------------------
//  MIT License
//  
//  Copyright (c) 2022 Global Graphics Software Ltd.
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
// -----------------------------------------------------------------------

using System.Buffers.Binary;
using System.Text;

namespace IppServer.Values;

public class IppDateTime : IppValue<DateTime>
{
    public IppDateTime(DateTime value) : base(value)
    {
    }

    public override void Encode(List<byte> buffer)
    {
        var bytes = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(bytes, 11);
        buffer.AddRange(bytes);

        BinaryPrimitives.WriteInt16BigEndian(bytes, (short) Value.Year);
        buffer.AddRange(bytes);

        var utc = Value.ToUniversalTime();
        buffer.Add((byte)utc.Month);
        buffer.Add((byte)utc.Day);
        buffer.Add((byte)utc.Hour);
        buffer.Add((byte)utc.Minute);
        buffer.Add((byte)utc.Second);
        buffer.Add((byte)Math.Floor(utc.Millisecond / 100d));
        buffer.Add((byte)'+');
        buffer.Add(0x00);
        buffer.Add(0x00);
    }

    public static IppDateTime Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        var year = BinaryPrimitives.ReadInt16BigEndian(buffer.Slice(offset + 2, 2));
        var month = buffer[offset + 4];
        var day = buffer[offset + 5];
        var hour = buffer[offset + 6];
        var minutes = buffer[offset + 7];
        var seconds = buffer[offset + 8];

        var direction = Encoding.UTF8.GetString(new[] {buffer[offset + 10]});
        var hoursFromUtc = buffer[offset + 11] * (direction.Equals("+") ? 1 : -1);

        offset += 13;

        var value = new DateTimeOffset(year, month, day, hour, minutes, seconds, new TimeSpan(0, hoursFromUtc, 0, 0));
        return new IppDateTime(value.UtcDateTime);
    }
}