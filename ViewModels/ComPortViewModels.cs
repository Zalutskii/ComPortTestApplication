using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ComPortTestApplication
{
    public class ComPortViewModels : INotifyPropertyChanged
    {
        private string _outgoingMessage;
        private byte[] _incomingMessage;
        private readonly SerialPort _serialPort;
        private ICommand _sendMessageCommand;
        private ICommand _closeOpenCommand;
        private bool _isAscii = true;

        public ComPortViewModels()
        {
            if (!PortNames.Any()) throw new ArgumentException("Нет ни одного COM порта");
            _serialPort = new SerialPort { Parity = Parity.None, DataBits = 8, StopBits = StopBits.One };
            PortName = PortNames.Last();
            PortSpeed = 9600;
            _serialPort.DataReceived += SerialPortOnDataReceived;
        }

        /// <summary>
        /// Обработчик полученого сообщения из COM порта 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var length = _serialPort.BytesToRead;
            IncomingMessage = new byte[length];
            _serialPort.Read(IncomingMessage, 0, length);
        }

        /// <summary>
        /// Название всех существующих COM портов
        /// </summary>
        public string[] PortNames { get; } = SerialPort.GetPortNames();

        /// <summary>
        /// Название COM порта к которому будет подключение
        /// </summary>
        public string PortName
        {
            get => _serialPort.PortName;
            set
            {
                if (_serialPort.PortName == value) return;
                _serialPort.PortName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOpen));
            }
        }
        /// <summary>
        /// Скорост порта
        /// </summary>
        public int PortSpeed
        {
            get => _serialPort.BaudRate;
            set
            {
                if (value <= 0) throw new ApplicationException("Значение не может быть меньше или равно нулю");
                if (_serialPort.BaudRate == value) return;
                _serialPort.BaudRate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Исходящее сообщение в ASCII или HEX
        /// </summary>
        public string OutgoingMessage
        {
            get => _outgoingMessage;
            set
            {
                if (_outgoingMessage == value) return;
                if (IsAscii)
                {
                    if (!AsciiValidation(value)) throw new ApplicationException("Содержит не ASCII символы");
                }
                else
                {
                    if (!HexValidation(value)) throw new ApplicationException("Содержит не HEX символы");
                }
                _outgoingMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OutgoingMessageByte));
            }
        }

        /// <summary>
        /// Исходящее сообщение в byte[]
        /// </summary>
        public byte[] OutgoingMessageByte => IsAscii ? Encoding.ASCII.GetBytes(_outgoingMessage) : String_To_Bytes(_outgoingMessage);

        /// <summary>
        /// Перевод HEX стороки в byte[]
        /// </summary>
        /// <param name="strInput">строка HEX</param>
        /// <returns></returns>
        private byte[] String_To_Bytes(string strInput)
        {
            var i = 0;
            var x = 0;
            var bytes = new byte[(strInput.Length) / 2]; 
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i += 2;
                ++x;
            }
            return bytes;
        }

        /// <summary>
        /// Входящее сообщение в byte[]
        /// </summary>
        public byte[] IncomingMessage
        {
            get => _incomingMessage;
            set
            {
                if (_incomingMessage == value) return;
                _incomingMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Признак ASCII или HEX если true ASCII
        /// </summary>
        public bool IsAscii
        {
            get => _isAscii;
            set
            {
                if (_isAscii == value) return;
                _isAscii = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OutgoingMessageByte));
            }
        }

        /// <summary>
        /// Валидация ASCII
        /// </summary>
        /// <param name="str">строка ASCII</param>
        /// <returns></returns>
        bool AsciiValidation(string str)
        {           
            foreach (var letter in str)
            {
                if (letter <= 127) continue;
                return false;               
            }
            return true;
        }

        /// <summary>
        /// Валидация HEX
        /// </summary>
        /// <param name="str">строка HEX</param>
        /// <returns></returns>
        public bool HexValidation(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        /// <summary>
        /// Признак открытого порта
        /// </summary>
        public bool IsOpen => _serialPort.IsOpen;

        /// <summary>
        /// Команда отправки сообщения
        /// </summary>
        public ICommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new Command(async () =>
        {
            await Task.Run(() => _serialPort.Write(OutgoingMessageByte, 0, OutgoingMessageByte.Length));
        }, () => IsOpen));

        /// <summary>
        /// Команда открытия/закрытия порта
        /// </summary>
        public ICommand CloseOpenCommand => _closeOpenCommand ?? (_closeOpenCommand = new Command(async () =>
        {
            await Task.Run(() =>
            {
                try
                {
                    if (IsOpen) _serialPort.Close();
                    else _serialPort.Open();
                }
                catch
                {
                    //-------
                }
                finally
                {
                    OnPropertyChanged(nameof(IsOpen));
                }
            });
        }));

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}