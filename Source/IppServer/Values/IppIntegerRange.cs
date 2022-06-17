// -----------------------------------------------------------------------
//  <copyright file="IppIntegerRange.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2022 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Buffers.Binary;

namespace IppServer.Values;

public class IppIntegerRange : IIppValue
{
    public void Encode(List<byte> buffer)
    {
        var length = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(length, 8);
        buffer.AddRange(length);

        var lowerBytes = new byte[4];
        var upperBytes = new byte[4];

        BinaryPrimitives.WriteInt32BigEndian(lowerBytes, Lower);
        BinaryPrimitives.WriteInt32BigEndian(lowerBytes, Upper);

        buffer.AddRange(lowerBytes);
        buffer.AddRange(upperBytes);
    }

    public static IppIntegerRange Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        var lower = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset + 2, 4));
        var upper = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset + 6, 4));

        offset += 10;

        return new IppIntegerRange(lower, upper);
    }

    public IppIntegerRange(int lower, int upper)
    {
        Lower = lower;
        Upper = upper;
    }

    public int Lower { get; }
    public int Upper { get; }
}