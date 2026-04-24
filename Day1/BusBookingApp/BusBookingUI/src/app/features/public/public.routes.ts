import { Routes } from '@angular/router';
import { Component } from '@angular/core';

@Component({
    selector: 'app-search',
    standalone: true,
    template: '<div class="p-8"><h1>Bus Search Page</h1><p>Search for your next journey here.</p></div>'
})
export class SearchComponent { }

export const PUBLIC_ROUTES: Routes = [
    { path: 'search', component: SearchComponent }
];
