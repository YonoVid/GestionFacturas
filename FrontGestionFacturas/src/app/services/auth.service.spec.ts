import { of } from 'rxjs';
import { IUserAuth } from '../models/interfaces/user-auth.interface';
import { IToken } from '../models/interfaces/user-token.interface';
import { IUser, UserRol } from '../models/interfaces/user.interface';

import { AuthService } from './auth.service';

describe('AuthService', () => {
  let httpClientSpy: {post: jasmine.Spy};
  let service: AuthService;

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['post']);
    service = new AuthService(httpClientSpy as any);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should be return IToken login data', (done: DoneFn) => {

    const mockUserCredentials: IUserAuth = {
      email: 'example@email.com',
      password: 'pwd'
    };

    const mockResultLogin: IToken = {
      token: {
        id: 0,
        token: 'keykeykeykeykeykey',
        refreshToken: '',
        guidId: 'guidguidguidguid',
        userName: 'example',
        emailId: 'example@email.com',
        userRol: UserRol.USER,
        validity: '',
        expireTime: new Date()
      },
      userName: 'example',
    }

    httpClientSpy.post.and.returnValue(of(mockResultLogin));

    const {email, password} = mockUserCredentials;
    service.login(email, password).subscribe((result) =>
    {
      expect(result).toEqual(mockResultLogin);
      done();
    });
  });

  it('should be return User registered data', (done: DoneFn) => {

    const mockUserCredentials: IUserAuth = {
      name: 'example',
      email: 'example@email.com',
      password: 'pwd'
    };

    const mockResultLogin = {
      id: 0,
    }

    httpClientSpy.post.and.returnValue(of(mockResultLogin));

    const {name, email, password} = mockUserCredentials;
    service.register(name!, email, password).subscribe((result) =>
    {
      expect(result).toEqual(mockResultLogin);
      done();
    });
  });
});
