using Hammer.Iiko.ReservePlugin.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammer.Iiko.ReservePlugin
{
    internal class ExternalReserveService : IObservable<ReserveDto>
    {
        private readonly List<IObserver<ReserveDto>> observers;

        public ExternalReserveService()
        {
            observers = new List<IObserver<ReserveDto>>();
        }

        public IDisposable Subscribe(IObserver<ReserveDto> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<ReserveDto>> _observers;
            private IObserver<ReserveDto> _observer;

            public Unsubscriber(List<IObserver<ReserveDto>> observers, IObserver<ReserveDto> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                    _observers.Remove(_observer);
            }
        }

        public void ReserveIncoming(ReserveDto reserve)
        {
            foreach(var observer in observers)
            {
                observer.OnNext(reserve);
            }
        }
    }
}
