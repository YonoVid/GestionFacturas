import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from "@angular/platform-browser"; 

@Pipe({
  name: 'safe'
})
export class SafePipe implements PipeTransform {

  constructor(private sanitizer: DomSanitizer) { }
  /**
   * Turn a URL to a safe resource.
   * @param url Url of the content to bypass security trust.
   * @returns A SafeResouceUrl that is allowed to be loaded.
   */
  transform(url: string) {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }
}

