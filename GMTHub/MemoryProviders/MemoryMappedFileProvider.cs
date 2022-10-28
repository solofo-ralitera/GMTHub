using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHub.MemoryProviders
{
    /**
     * Copied from https://github.com/RenCloud/scs-sdk-plugin
     * */
    internal class MemoryMappedFileProvider
    {
        private MemoryMappedFile _memoryMappedHandle;

        private MemoryMappedViewAccessor _memoryMappedView;

        private const uint defaultMapSize = 32 * 1024;

        public Exception HookException { get; private set; }
        public byte[] RawData { get; private set; }

        public bool Hooked { get; private set; }

        public void Connect(string map, uint mapSize = defaultMapSize)
        {
            if (Hooked)
            {
                Disconnect();
            }

            // Reset any errors
            HookException = null;

            try
            {
                RawData = new byte[mapSize];

                _memoryMappedHandle = MemoryMappedFile.CreateOrOpen(map, mapSize, MemoryMappedFileAccess.ReadWrite);
                _memoryMappedView = _memoryMappedHandle.CreateViewAccessor(0, mapSize);

                // Mark as a success.
                Hooked = true;
            }
            catch (Exception e)
            {
                Hooked = false;
                HookException = e;
            }
        }

        public void Disconnect()
        {
            Hooked = false;

            _memoryMappedView.Dispose();
            _memoryMappedHandle.Dispose();
        }

        /// <summary>
        ///     reread data from memory view
        /// </summary>
        public void Update()
        {
            if (!Hooked || _memoryMappedView == null)
            {
                return;
            }

            // Re-read data from the view.
            _memoryMappedView.ReadArray(0, RawData, 0, RawData.Length);
        }
    }
}
