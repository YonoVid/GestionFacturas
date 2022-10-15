import { of } from 'rxjs';
import { take } from 'rxjs/operators';
import { TestBed } from '@angular/core/testing';
import { DomSanitizer } from '@angular/platform-browser';
import { SafePipe } from './safe.pipe';
import { SecurityContext } from '@angular/core';

describe('SafePipe', () => {
  let domSanitizerMock: DomSanitizer;

  beforeEach(() => {
    domSanitizerMock = {
      sanitize: () => 'safeString',
      bypassSecurityTrustHtml: () => 'safeString',
    } as any;
  });

  it('create an instance', () => {
    const service: DomSanitizer = TestBed.inject(DomSanitizer);

    const pipe = new SafePipe(service);
    expect(pipe).toBeTruthy();
  });

  it('should return value "safeString" ', () => {
    const service: DomSanitizer = TestBed.inject(DomSanitizer);
    const pipe = new SafePipe(service);

    const pipeValue = pipe.transform('Cross <script>alert(\'Hello\')</script>');
    const sanitizedValue = service.sanitize(SecurityContext.RESOURCE_URL,
                                            pipeValue);

    expect(sanitizedValue).toEqual('Cross <script>alert(\'Hello\')</script>');
  });
});
