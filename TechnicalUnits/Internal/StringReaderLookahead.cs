using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalUnits.Internal;

internal class StringReaderLookahead : StringReader
{
    private readonly List<int> peekList = new List<int>();

    public StringReaderLookahead(string str)
        : base(str)
    {
    }

    #region Overrides of TextReader

    public override int ReadBlock(char[] buffer, int index, int count)
    {
        if (peekList.Count == 0)
            return base.ReadBlock(buffer, index, count);
        return Read(buffer, index, count);
    }

    #endregion

    #region Overrides of StringReader

    public override int Peek()
    {
        if (peekList.Count < 1)
            return base.Peek();

        return peekList.First();
    }

    public int Peek(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Negative indices are not supported.");
        if (index == 0)
            return Peek();
        if (index < peekList.Count)
            return peekList[index];
        if (index == peekList.Count)
            return base.Peek();

        // Index lies beyond the current list + base.Peek() -> read from base until base.Peek() is the requested index
        while (peekList.Count < index)
            peekList.Add(base.Read());

        return base.Peek();
    }

    public string PeekStr(int length)
    {
        var str = new StringBuilder(length);
        for (var i = 0; i < length; i++)
            str[i] = (char) Peek(i);

        return str.ToString();
    }

    public string ReadStr(int length)
    {
        var str = new StringBuilder(length);
        str.Length = length;
        for (var i = 0; i < length; i++)
            str[i] = (char) Read();

        return str.ToString();
    }

    public override int Read()
    {
        if (peekList.Count < 1)
            return base.Read();

        var cnt = peekList.First();
        peekList.RemoveAt(0);

        return cnt;
    }

    public override int Read(char[] buffer, int index, int count)
    {
        var cnt = 0;
        for (var i = index; i < index + count; i++)
        {
            var ch = Read();
            if (ch > -1)
            {
                buffer[i] = (char) ch;
                cnt++;
            }
            else
                return cnt;
        }

        return cnt;
    }

    public override string ReadToEnd()
    {
        return String.Join(String.Empty, peekList) + base.ReadToEnd();
    }

    public override string ReadLine()
    {
        var line = new StringBuilder();
        var ch = Read();
        while (ch != '\n' && ch != '\r' && ch > -1)
        {
            line.Append((char) ch);
            ch = Read();
        }
        if (ch == '\r' && Peek() == '\n')
            Read(); // Read the \n from a \r\n line break

        return line.ToString();
    }

    public override Task<string> ReadLineAsync()
    {
        if (peekList.Count == 0)
            return base.ReadLineAsync();

        return Task.FromResult(ReadLine());
    }

    public override Task<string> ReadToEndAsync()
    {
        if (peekList.Count == 0)
            return base.ReadToEndAsync();

        return Task.FromResult(ReadToEnd());
    }

    public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
    {
        if (peekList.Count == 0)
            return base.ReadBlockAsync(buffer, index, count);

        return Task.FromResult(ReadBlock(buffer, index, count));
    }

    public override Task<int> ReadAsync(char[] buffer, int index, int count)
    {
        if (peekList.Count == 0)
            return base.ReadAsync(buffer, index, count);

        return Task.FromResult(Read());
    }

    #endregion
}
