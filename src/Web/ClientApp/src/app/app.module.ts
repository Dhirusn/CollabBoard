import { BrowserModule } from '@angular/platform-browser';
import { APP_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { ModalModule } from 'ngx-bootstrap/modal';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { TodoComponent } from './todo/todo.component';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

const routes: Routes = [
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'todo', component: TodoComponent },
            { path: 'board-list',loadChildren: () => import('./features/board-list/board-list.module').then(m => m.BoardListModule) },
            { path: 'board', loadChildren: () => import('./features/board/board.module').then(m => m.BoardModule) }
        ]
@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        CounterComponent,
        FetchDataComponent,
        TodoComponent
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        FormsModule,
        RouterModule.forRoot(routes, { initialNavigation: 'enabledBlocking' }),
        BrowserAnimationsModule,
        ModalModule.forRoot()],
    providers: [
        { provide: APP_ID, useValue: 'ng-cli-universal' },
        { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
        provideHttpClient(withInterceptorsFromDi())
    ]
})
export class AppModule { }
