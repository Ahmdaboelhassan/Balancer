import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin, map, Observable } from 'rxjs';

export interface TransfersRates {
  usdToEgp: number;

  gold18EgpPerGram: number;
  gold20EgpPerGram: number;
  gold22EgpPerGram: number;
  gold24EgpPerGram: number;

  silverEgpPerGram: number;

  btcEgp: number;
}

interface GoldApiResponse {
  price: number;
  symbol: string;
  currency: string;
  timestamp: number;
}

interface ExchangeRateResponse {
  base: string;
  quote: string;
  rate: number;
}

@Injectable({
  providedIn: 'root'
})
export class RatesService {

  private readonly goldApiUrl = 'https://api.gold-api.com/price';
  private readonly frankfurterUrl = 'https://api.frankfurter.dev/v2';

  private readonly TROY_OUNCE_GRAMS = 31.1034768;

 constructor(private http: HttpClient) {}
 getRates(): Observable<TransfersRates> {

    return forkJoin({
      gold: this.http.get<GoldApiResponse>(
        `${this.goldApiUrl}/XAU`
      ),

      silver: this.http.get<GoldApiResponse>(
        `${this.goldApiUrl}/XAG`
      ),

      bitcoin: this.http.get<GoldApiResponse>(
        `${this.goldApiUrl}/BTC`
      ),

      usdEgp: this.http.get<ExchangeRateResponse>(
        `${this.frankfurterUrl}/rate/USD/EGP`
      )
    }).pipe(

      map(({ gold, silver, bitcoin, usdEgp }) => {

        const usdToEgp = usdEgp.rate;

        // XAU price is per troy ounce
        const gold24UsdPerGram =
          gold.price / this.TROY_OUNCE_GRAMS;

        const gold24EgpPerGram =
          gold24UsdPerGram * usdToEgp;

        // XAG price is per troy ounce
        const silverUsdPerGram =
          silver.price / this.TROY_OUNCE_GRAMS;

        const silverEgpPerGram =
          silverUsdPerGram * usdToEgp;

        // BTC price is already per BTC
        const btcEgp =
          bitcoin.price * usdToEgp;

        return {
          usdToEgp,

          gold24EgpPerGram:
            gold24EgpPerGram,

          gold22EgpPerGram:
            gold24EgpPerGram * 22 / 24,

          gold20EgpPerGram:
            gold24EgpPerGram * 20 / 24,

          gold18EgpPerGram:
            gold24EgpPerGram * 18 / 24,

          silverEgpPerGram,

          btcEgp
        };
      })
    );
  }
}