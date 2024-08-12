//тип данных
export type LoginData = {
    phoneNumber: string,
    password: string
}

export class UserLogin {
    phoneNumber: string;
    password: string;

    constructor(data: LoginData) {
        this.phoneNumber = data.phoneNumber;
        this.password = data.password;
    }
}