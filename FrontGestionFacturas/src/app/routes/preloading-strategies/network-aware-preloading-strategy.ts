import { Injectable } from '@angular/core';
import { PreloadingStrategy, Route } from '@angular/router';
import { Observable, EMPTY } from 'rxjs';

// avoid typing issues for now
export declare var navigator: any;

@Injectable({ providedIn: 'root' })
export class NetworkAwarePreloadStrategy implements PreloadingStrategy {
    preload(route: Route, load: () => Observable<any>): Observable<any> {
        return this.hasGoodConnection() ? load() : EMPTY;
    }
    /**
     * Determines if the connection or configuration of the user
     * allows to preload the content. 
     * @returns Boolean indicating if the data should be preloaded.
     */
    hasGoodConnection(): boolean {
        const conn = navigator.connection;
        if (conn) {
            if (conn.saveData) {
                return false; // save data mode is enabled, so dont preload
            }
            const avoidTheseConnections = ['slow-2g', '2g' /* , '3g', '4g' */];
            const effectiveType = conn.effectiveType || '';
            if (avoidTheseConnections.includes(effectiveType)) {
                return false;
            }
        }
        return true;
    }
}