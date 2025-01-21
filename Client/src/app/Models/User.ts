export class User {
  constructor(
    public Username: string,
    private _token: string,
    private _expireOn: Date
  ) {}

  get getToken() {
    let dateBetween = new Date(this._expireOn).getTime() - new Date().getTime();
    if (dateBetween <= 0) return null;
    return this._token;
  }
}
