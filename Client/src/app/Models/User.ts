export class User {
  constructor(
    public Username: string,
    private _token: string,
    private _expireOn: Date,
    private _refreshToken: string,
    private _refreshTokenExpireOn: Date
  ) {}

  get getToken() {
    let dateBetween = new Date(this._expireOn).getTime() - new Date().getTime();
    if (dateBetween <= 0) return null;
    return this._token;
  }

  get expireOnHours() {
    let dateBetween = new Date(this._expireOn).getTime() - new Date().getTime();
    return dateBetween / (60 * 60 * 1000);
  }

  get getRefreshToken() {
    let dateBetween =
      new Date(this._refreshTokenExpireOn).getTime() - new Date().getTime();
    if (dateBetween <= 0) return null;
    return this._refreshToken;
  }
}
