using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalUnits.Internal;

internal class StringReaderLookahead : StringReader
{
    private readonly List<int> _peekList = [];

    public StringReaderLookahead(string str)
        : base(str)
    {
    }

    #region Overrides of TextReader

    public override int ReadBlock(char[] buffer, int index, int count)
    {
        if (_peekList.Count == 0)
            return base.ReadBlock(buffer, index, count);

        return Read(buffer, index, count);
    }

    #endregion

    #region Overrides of StringReader

    public override int Peek()
    {
        if (_peekList.Count < 1)
            return base.Peek();

        return _peekList[0];
    }

    public int Peek(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Negative indices are not supported.");
        if (index == 0)
            return Peek();
        if (index < _peekList.Count)
            return _peekList[index];
        if (index == _peekList.Count)
            return base.Peek();

        // Index lies beyond the current list + base.Peek()
        // -> read from base until base.Peek() is the requested index
        while (_peekList.Count < index)
            _peekList.Add(base.Read());

        return base.Peek();
    }

    public string PeekStr(int length)
    {
        var str = new StringBuilder(length);
        str.Length = length;
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
        if (_peekList.Count < 1)
            return base.Read();

        var cnt = _peekList[0];
        _peekList.RemoveAt(0);

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

    public override string ReadToEnd() => String.Join(String.Empty, _peekList) + base.ReadToEnd();

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
        if (_peekList.Count == 0)
            return base.ReadLineAsync();

        return Task.FromResult(ReadLine());
    }

    public override Task<string> ReadToEndAsync()
    {
        if (_peekList.Count == 0)
            return base.ReadToEndAsync();

        return Task.FromResult(ReadToEnd());
    }

    public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
    {
        if (_peekList.Count == 0)
            return base.ReadBlockAsync(buffer, index, count);

        return Task.FromResult(ReadBlock(buffer, index, count));
    }

    public override Task<int> ReadAsync(char[] buffer, int index, int count)
    {
        if (_peekList.Count == 0)
            return base.ReadAsync(buffer, index, count);

        return Task.FromResult(Read(buffer, index, count));
    }

    #endregion
}
